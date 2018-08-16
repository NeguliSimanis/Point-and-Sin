using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileRange : MonoBehaviour
{
    private bool isPlayerMinion;
    [SerializeField] EnemyController enemyController;

    private void Start()
    {
        if (enemyController == null)
            enemyController = transform.parent.gameObject.GetComponent<EnemyController>();

        isPlayerMinion = enemyController.isPlayerMinion;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy" && isPlayerMinion)
        {
            enemyController.StandbyToShootTarget();
        }
        else if ((col.gameObject.tag == "Player" || col.gameObject.tag == "PlayerMinion") && !isPlayerMinion)
        {
            enemyController.StandbyToShootTarget();
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy" && isPlayerMinion)
        {
            enemyController.StopStandbyToShootTarget();
        }
        else if ((col.gameObject.tag == "Player" || col.gameObject.tag == "PlayerMinion") && !isPlayerMinion)
        {
            enemyController.StopStandbyToShootTarget();
        }
    }
    
}
