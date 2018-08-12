using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActiveState : MonoBehaviour {

	public void Toggle()
    {
        bool isActive = gameObject.activeInHierarchy;
        gameObject.SetActive(!isActive);
    }
}
