using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoPanel : MonoBehaviour
{

	public void DisplayItemInfo(bool display = true)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(display);
        }
        if (display)
        {
            Debug.Log("displaying item info");
           
        }
        else
        {
            Debug.Log("hiding item info");
        }
    }
}
