﻿using System.Collections;
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

    public void StartFireball(bool isFlyingRight)
    {
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
        Debug.Log("EXPLODE");
        animator.SetTrigger("explode");
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(explosionDuration);
        Destroy(gameObject);
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
