using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour

{
    bool displayStats = false;
    Text thisText;

    private void Start()
    {
        thisText = gameObject.GetComponent<Text>();
    }

    void OnEnable()
    {
        displayStats = true;
    }

    private void OnDisable()
    {
        displayStats = false;   
    }
    // Update is called once per frame
    void Update ()
    {
		if (displayStats)
        {
            thisText.text = "LEVEL: " + PlayerData.current.currentLevel.ToString() +  "\n SOULS PURGED: " + PlayerData.current.enemiesKilled.ToString();
        }
	}
}
