using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerInteraction : MonoBehaviour
{

    [SerializeField] PlayerController playerController;
    [SerializeField] EnemyController enemyController;

    private void OnMouseDown()
    {
        //Debug.Log("Enemy " + enemyController.enemyID + " targeted");
        playerController.TargetEnemy(enemyController.enemyID, enemyController);
    }
}
