using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileRange : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
            enemyController.StandbyToShootPlayer();
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
            enemyController.StopStandbyToShootPlayer();
    }
    
}
