using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

    public int projectileDamage;
    private float projectileDuration = 0.3f;
    private float projectileDeathTime;          // automatically explodes after this duration
    private float projectileMoveSpeed = 0.7f;
    bool projectileStarted = false;
    bool flyingRight;
    bool isExploding = false;

    float explosionDuration;
    [SerializeField] AnimationClip explosionAnimation;
    [SerializeField] Animator animator;

    // AUDIO
    [SerializeField] AudioSource audioControl;
    [SerializeField] AudioClip projectileSFX;
    [SerializeField] AudioClip explosionSFX;


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
        isExploding = true;
        animator.SetTrigger("explode");
        audioControl.PlayOneShot(explosionSFX, 1F);
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(explosionDuration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //collision.gameObject.GetComponent<EnemyController>().TakeDamage(PlayerData.current.fireballDamage);
            //Debug.Log("player hit!");
            PlayerData.current.DamagePlayer(projectileDamage);
            Explode();
        }
    }

    void MoveProjetile()
    {
        if (isExploding)
            return;
        if (flyingRight)
            transform.position = new Vector2(transform.position.x, transform.position.y) + Vector2.right * projectileMoveSpeed * Time.deltaTime;
        else
            transform.position = new Vector2(transform.position.x, transform.position.y) + Vector2.left * projectileMoveSpeed * Time.deltaTime;
    }
}
