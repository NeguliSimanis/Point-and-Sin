using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script calls player movement in PlayerControl.cs when you click on a walkable area
/// </summary>
public class PlayerClickArea : MonoBehaviour
{
    [SerializeField]
    Button[] clickableAreas; // don't call walk function if you click outside of this area

    [SerializeField]
    PlayerController playerController;

    private void Start()
    {
        foreach (Button clickableArea in clickableAreas)
        {
            clickableArea.onClick.AddListener(CallPlayerWalkCommand);
        }
    }

    private void CallPlayerWalkCommand()
    {
        playerController.CheckIfPlayerIsWalking();
    }
}
