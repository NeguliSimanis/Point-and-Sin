using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

    private float fireBallDamage;
    private float fireBallDuration = 2f;
    private float fireBallDeathTime;
    private float fireBallMoveSpeed = 1f;

    #region MOVEMENT variables
    public Vector2 targetPosition;
    private Vector2 dirNormalized;
    private Transform targetTransform;
    #endregion

   /* public Fireball(Transform target)
    {
        targetPosition = target.position;
    }*/

    private void Start()
    {
        //Debug.Log("FIRE!");
        GetTargetPositionAndDirection();
        fireBallDeathTime = fireBallDuration + Time.time;
    }

    private void Update()
    {
        MoveFireball();
        if (Time.time > fireBallDuration + fireBallDeathTime)
            Explode();
    }

    private void Explode()
    {
       // Debug.Log("destroyed" + Time.time);
        Destroy(gameObject);
    }

    void GetTargetPositionAndDirection()
    {
        dirNormalized = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
        dirNormalized = dirNormalized.normalized;
    }

    void MoveFireball()
    {
        Debug.Log("moving");
        transform.position = new Vector2(transform.position.x, transform.position.y) + dirNormalized * fireBallMoveSpeed * Time.deltaTime;
    }
}
