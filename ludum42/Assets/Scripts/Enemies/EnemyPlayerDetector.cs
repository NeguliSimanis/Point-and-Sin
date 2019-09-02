using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects if the player has entered the enemy's attacking range
/// </summary>
public class EnemyPlayerDetector : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            enemyController.StandbyToMeleeAttackPlayer();
        }

    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            enemyController.StopStandbyToMeleeAttackPlayer();
        }
    }
}
