using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

    private float fireBallDamage;
    private float fireBallDuration = 0.4f;
    private float fireBallDeathTime;
    private float fireBallMoveSpeed = 2f;

    #region CURRENT STATE
    bool fireBallStarted = false;
    bool flyingRight;
    bool isExploding = false;
    #endregion

    #region EXPLOSION
    float explosionDuration;
    [SerializeField] AnimationClip explosionAnimation;
    [SerializeField] Animator animator;
    #endregion

    #region AUDIO
    private float fireBallSFXVolume = 1.2f;
    private float fireBallExplodeSFXVolume = 1.5f;

    [SerializeField] AudioSource audioControl;
    [SerializeField] AudioClip fireballSFX;
    [SerializeField] AudioClip explosionSFX;
    #endregion

    private void Start()
    {
        audioControl = GameObject.Find("Audio").GetComponent<AudioSource>();
    }

    public void StartFireball(bool isFlyingRight)
    {
        audioControl = GameObject.Find("Audio").GetComponent<AudioSource>();
        audioControl.PlayOneShot(fireballSFX, fireBallSFXVolume);
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
        audioControl.PlayOneShot(explosionSFX, fireBallExplodeSFXVolume);
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
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(PlayerData.current.fireballDamage, DamageSource.PlayerFireball);
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
