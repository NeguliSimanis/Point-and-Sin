using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    PlayerController playerController;

	void Start ()
    {
        playerController = gameObject.GetComponent<PlayerController>();
	}	

	void Update ()
    {
        if (playerController.isWaitingToMove)
        {
            playerController.CheckIfPlayerIsWalking();
        }
        
        if (playerController.isWalking)
        {
            playerController.CheckIfPlayerIsWalking();
            MovePlayer();
        }
	}

    private void MovePlayer()
    {
        transform.position = new Vector2
            (transform.position.x, transform.position.y) + 
            playerController.dirNormalized * 
            PlayerData.current.moveSpeed * 
            Time.deltaTime;
    }
}
