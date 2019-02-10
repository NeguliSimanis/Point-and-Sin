using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectAfterKeyPress : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKey)
        {
            gameObject.SetActive(false);
        }
    }
}
