using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

    private float fireBallDamage;
    private float fireBallDuration = 0.3f;
    private float fireBallDeathTime;
    private float fireBallMoveSpeed = 1f;
    bool fireBallStarted = false;
    bool flyingRight;
    bool isExploding = false;

    float explosionDuration;
    [SerializeField] AnimationClip explosionAnimation;
    [SerializeField] Animator animator;

    // AUDIO
    [SerializeField] AudioSource audioControl;
    [SerializeField] AudioClip fireballSFX;
    [SerializeField] AudioClip explosionSFX;

    private void Start()
    {
        audioControl = GameObject.Find("Audio").GetComponent<AudioSource>();
    }

    public void StartFireball(bool isFlyingRight)
    {
        audioControl = GameObject.Find("Audio").GetComponent<AudioSource>();
        audioControl.PlayOneShot(fireballSFX, 0.6F);
        fireBallStarted = true;
        flyingRight = isFlyingRight;
        fireBallDeathTime = fireBallDuration + Time.time;
        explosionDuration = explosionAnimation.length;
    }

    private void Update()
    {
        if (!fireBallStarted)
        {
            return;
        }
        MoveFireball();
        if (Time.time > fireBallDuration + fireBallDeathTime)
            Explode();
    }

    private void Explode()
    {
        if (isExploding)
            return;
        isExploding = true;
        animator.SetTrigger("explode");
        Debug.Log("EXPLODE");
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
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(PlayerData.current.fireballDamage);
            Explode();
        }
    }

    void MoveFireball()
    {
        if (isExploding)
            return;
        if (flyingRight)
            transform.position = new Vector2(transform.position.x, transform.position.y) + Vector2.right * fireBallMoveSpeed * Time.deltaTime;
        else
            transform.position = new Vector2(transform.position.x, transform.position.y) + Vector2.left * fireBallMoveSpeed * Time.deltaTime;
    }
}
