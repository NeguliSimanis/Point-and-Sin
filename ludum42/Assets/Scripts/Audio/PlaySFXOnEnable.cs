using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays a random sound from a given array when this component is enabled
///
/// </summary>
public class PlaySFXOnEnable : MonoBehaviour
{

    [SerializeField]
    PlayerSFX playerSFXController;

    private void OnEnable()
    {
        playerSFXController.PlayWalkingSFX();
    }
}
