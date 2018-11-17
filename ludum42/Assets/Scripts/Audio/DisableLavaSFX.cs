using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableLavaSFX : MonoBehaviour {

    [SerializeField]
    LavaSounds lavaSounds;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            lavaSounds.DisableLavaSFX();
        }
    }
}
