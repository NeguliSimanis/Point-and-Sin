using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Re-enables checking player input, thus letting you control the character
/// </summary>
public class UnfreezePlayer : MonoBehaviour
{

    [SerializeField]
    bool unfreezeOnDisableObject = true;

    [SerializeField]
    float unfreezeDelay = 0.3f;

    PlayerController playerController;
	void Start ()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}

    private void OnDisable()
    {
        Debug.Log("SCRIPT DISABLED");
        if (unfreezeOnDisableObject)
        {
            playerController.UnfreezeMovement(unfreezeDelay);
        }
    }
}
