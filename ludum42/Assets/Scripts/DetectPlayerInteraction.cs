using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Detects if player clicks on enemy or hovers above enemy
/// </summary>
public class DetectPlayerInteraction : MonoBehaviour
{

    [SerializeField] PlayerController playerController;
    [SerializeField] EnemyController enemyController;

    private void Start()
    {
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnMouseDown()
    {
        //Debug.Log("Enemy " + enemyController.enemyID + " targeted");
        playerController.TargetEnemy(enemyController.enemyID, enemyController);
    }

    /// <summary>
    /// Display enemy HP bar on mouse hover
    /// </summary>
    private void OnMouseOver()
    {
        // boss hp bar is triggered by being noticed by the boss instead
        if (enemyController.GetEnemyType() != EnemyController.EnemyType.SkullBoss)
        {
            // don't display player minion hp bars
            if (!enemyController.isPlayerMinion)
            {
                playerController.lastHoveredEnemy = enemyController;
                playerController.isMouseOverEnemy = true;
            }
        }
    }

    private void OnMouseExit()
    {
        if (enemyController.GetEnemyType() != EnemyController.EnemyType.SkullBoss)
            playerController.isMouseOverEnemy = false;
    }
}
