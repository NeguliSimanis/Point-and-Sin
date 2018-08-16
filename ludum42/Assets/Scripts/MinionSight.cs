using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSight : MonoBehaviour
{
    EnemyController minionController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (minionController == null)
        {
            minionController = transform.parent.gameObject.GetComponent<EnemyController>();
        }
        if (collision.gameObject.tag == "Enemy")
        {
            minionController.isNearTargetEnemy = true;
            minionController.otherEnemyTransform = collision.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (minionController == null)
        {
            minionController = transform.parent.gameObject.GetComponent<EnemyController>();
        }
        if (collision.gameObject.tag == "Enemy")
        {
            minionController.isNearTargetEnemy = false;
        }
    }

}
