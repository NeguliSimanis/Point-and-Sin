using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Switches game object between active and inactive state.
/// </summary>
public class ToggleActiveState : MonoBehaviour {

	public void Toggle()
    {
        bool isActive = gameObject.activeInHierarchy;
        gameObject.SetActive(!isActive);
    }
}
