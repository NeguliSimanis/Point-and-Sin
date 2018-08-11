using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects if the player is close enough to attack enemy
/// </summary>

public class PlayerEnemyDetector : MonoBehaviour {

    [SerializeField] PlayerController playerController;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            playerController.isNearEnemy = true;
            int enemyID = col.gameObject.GetComponent<EnemyController>().enemyID;
            playerController.nearEnemyID = enemyID;

            if (playerController.targetEnemyID == enemyID)
            {
                playerController.TargetEnemy(enemyID, col.gameObject.GetComponent<EnemyController>());
            }
            //Debug.Log("entering melee zone of enemy " + enemyID);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            //Debug.Log("exiting melee zone");
            playerController.isNearEnemy = false;
            playerController.nearEnemyID = -1;
        }
    }
}
