using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

    public enum ProjectileType { Succubus, SkullBoss}

    public int projectileDamage;
    [SerializeField] private float projectileDuration = 0.3f;
    private float projectileDeathTime;          // automatically explodes after this duration
    private float projectileMoveSpeed = 0.7f;
    bool projectileStarted = false;
    bool flyingRight;
    bool isExploding = false;
    int bossChildProjectileID = -1; // if ID == -1, then this is regular projectile

    [SerializeField] ProjectileType type;
    [SerializeField] GameObject childProjectile; // spawned for boss projectiles

    // ANIMATION
    float explosionDuration;
    [SerializeField] AnimationClip explosionAnimation;
    [SerializeField] Animator animator;
    
    // AUDIO
    [SerializeField] AudioSource audioControl;
    [SerializeField] AudioClip projectileSFX;
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] AudioClip bossDamagesPlayerSFX;
    float bossDamagesPlayerSFXVolume = 1f;


    private void Start()
    {
        audioControl = GameObject.Find("Audio").GetComponent<AudioSource>();
    }

    public void StartProjectile(bool isFlyingRight, int damage)
    {
        projectileStarted = true;
        flyingRight = isFlyingRight;
        projectileDamage = damage;

        // AUDIO
        audioControl = GameObject.Find("Audio").GetComponent<AudioSource>();
        audioControl.PlayOneShot(projectileSFX, 0.9F);
        
        projectileDeathTime = projectileDuration + Time.time;
        explosionDuration = explosionAnimation.length;
    }

    public void StartChildProjectile(bool isFlyingRight, int damage, int projectileID)
    {
        bossChildProjectileID = projectileID;
        StartProjectile(isFlyingRight, damage);
    }

    private void Update()
    {
        if (!projectileStarted)
        {
            return;
        }
        MoveProjetile();
        if (Time.time > projectileDuration + projectileDeathTime)
            Explode();
    }

    private void Explode()
    {
        if (isExploding)
            return;

        // regular projectile stuff
        isExploding = true;
        animator.SetTrigger("explode");
        audioControl.PlayOneShot(explosionSFX, 1F);

        // self destruct
        StartCoroutine(SelfDestruct());
    }

    // main boss projectile spawns children
    void SpawnChildProjectiles()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject projectile = Instantiate(childProjectile, transform.position, transform.rotation);
            projectile.GetComponent<EnemyProjectile>().StartChildProjectile(flyingRight, (int)(projectileDamage / 2), i);
            projectile.transform.parent = null;
        }
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(explosionDuration);
        // boss projectile spawns children
        if (type == ProjectileType.SkullBoss)
        {
            SpawnChildProjectiles();
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerData.current.DamagePlayer(projectileDamage);
            if (type == ProjectileType.SkullBoss || bossChildProjectileID > -1)
                audioControl.PlayOneShot(bossDamagesPlayerSFX, bossDamagesPlayerSFXVolume);
            Explode();
        }
    }

    void MoveProjetile()
    {
        if (isExploding)
            return;
        if (flyingRight)
        {
            // regular projectile or second boss child projectile
            if (bossChildProjectileID == -1 || bossChildProjectileID == 1)
                transform.position = new Vector2(transform.position.x, transform.position.y) + Vector2.right * projectileMoveSpeed * Time.deltaTime;
            // boss child projectiles
            else if (bossChildProjectileID == 0)
                transform.position = new Vector2(transform.position.x, transform.position.y) + new Vector2(0.5f,0.5f) * projectileMoveSpeed * Time.deltaTime;
            else if (bossChildProjectileID == 2)
                transform.position = new Vector2(transform.position.x, transform.position.y) + new Vector2(0.5f, -0.5f) * projectileMoveSpeed * Time.deltaTime;
        }
        else
        {

            // regular projectile or second boss projectile
            if (bossChildProjectileID == -1 || bossChildProjectileID == 1)
                transform.position = new Vector2(transform.position.x, transform.position.y) + Vector2.left * projectileMoveSpeed * Time.deltaTime;
            // boss child projectiles
            else if (bossChildProjectileID == 0)
                transform.position = new Vector2(transform.position.x, transform.position.y) + new Vector2(-0.5f, 0.5f) * projectileMoveSpeed * Time.deltaTime;
            else if (bossChildProjectileID == 2)
                transform.position = new Vector2(transform.position.x, transform.position.y) + new Vector2(-0.5f, -0.5f) * projectileMoveSpeed * Time.deltaTime;
        }
    }
}
