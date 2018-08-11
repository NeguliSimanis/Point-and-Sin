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
            Debug.Log("near enemy");
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            playerController.isNearEnemy = false;
            Debug.Log("far enemy");
        }
    }
}
