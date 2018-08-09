using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region DATA
    [SerializeField] private float moveSpeed;
    #endregion

    #region MOVEMENT
    private Vector2 targetPosition;
    private Vector2 dirNormalized;
    float timeToReachTarget;
    #endregion

    #region COMPONENTS
    Rigidbody2D rigidBody2D;
    #endregion

    void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetTargetPositionAndDirection();
        }
        if (Vector2.Distance(targetPosition, transform.position) <= 0.01f)
        {
            return; 
        }
        else
        {
            MovePlayer(); 
        }
    }

    void GetTargetPositionAndDirection()
    {
        targetPosition = Input.mousePosition;
        targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
        dirNormalized = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
        dirNormalized = dirNormalized.normalized;
    }

    void MovePlayer()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y) + dirNormalized * moveSpeed * Time.deltaTime;
    }
}
