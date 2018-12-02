using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

    public enum ProjectileType { Succubus, SkullBoss}

    #region DATA
    [SerializeField] ProjectileType type;
    [SerializeField] private float projectileDuration = 0.4f;
    public int projectileDamage;
    private float projectileDeathTime;          // automatically explodes after this duration
    private float projectileMoveSpeed = 1.5f;
    #endregion

    #region PROJECTILE STATE
    public bool isMinionProjectile = false;
    bool projectileStarted = false;
    bool flyingRight;
    bool isExploding = false;
    #endregion

    #region BOSS PROJECTILE
    [SerializeField] GameObject childProjectile; // spawned for boss projectiles
    int bossChildProjectileID = -1;             // if ID == -1, then this is regular projectile
    #endregion

    #region ANIMATION
    float explosionDuration;
    [SerializeField] AnimationClip explosionAnimation;
    [SerializeField] Animator animator;
    #endregion

    #region AUDIO
    [SerializeField] AudioSource audioControl;
    [SerializeField] AudioClip projectileSFX;
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] AudioClip bossDamagesPlayerSFX;
    float bossDamagesPlayerSFXVolume = 1f;
    #endregion

    private void Start()
    {
        audioControl = GameObject.Find("Audio").GetComponent<AudioSource>();
        // set minion projectile color to different
        if (isMinionProjectile)
        {
            animator.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4117647f, 1, 0.5094506f);
        }
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
        // this is a regular enemy projectile attacking player - damage player
        if (collision.gameObject.tag == "Player" && !isMinionProjectile)
        {
            PlayerData.current.DamagePlayer(projectileDamage);
            if (type == ProjectileType.SkullBoss || bossChildProjectileID > -1)
                audioControl.PlayOneShot(bossDamagesPlayerSFX, bossDamagesPlayerSFXVolume);
            Explode();
        }
        // this is a minion projectile attacking enemy - damage enemy
        else if (collision.gameObject.tag == "Enemy" && isMinionProjectile)
        {
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(projectileDamage, DamageSource.EnemyProjectile);
            Explode();
        }
        // this is a regular enemy projectile attacking minion - damage minion
        else if (collision.gameObject.tag == "PlayerMinion" && !isMinionProjectile)
        {
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(projectileDamage, DamageSource.MinionProjectile);
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
