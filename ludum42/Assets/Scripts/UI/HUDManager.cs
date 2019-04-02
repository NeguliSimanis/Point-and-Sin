using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    bool isAtLeastOneMenuActive = false;
    List<GameObject> activeHuds = new List<GameObject>();
    List<GameObject> openHudButtons = new List<GameObject>();

    public void OpenHudMenu(GameObject hudMenu)
    {
        activeHuds.Add(hudMenu);
        
        isAtLeastOneMenuActive = true;
    }

    /// <summary>
    /// Called by button press at the same time as OpenHUDMenu
    /// </summary>
    /// <param name="openHudButton">the button that was used to open HUD menu</param>
    public void StoreOpenHudButton(GameObject openHudButton)
    {
        openHudButtons.Add(openHudButton);
    }

    public bool CheckIfHudOpen()
    {
        if (isAtLeastOneMenuActive)
            return true;
        return false;
    }

    public void CloseActiveHUD()
    {
        foreach (GameObject hud in activeHuds)
        {
            hud.SetActive(false);
        }
        activeHuds.Clear();
        isAtLeastOneMenuActive = false;
        ShowOpenHudButtons();
    }

    /// <summary>
    /// buttons that open HUD are set to inactive when they are pressed. 
    /// This method resets them to active once HUD menus are hidden
    /// </summary>
    public void ShowOpenHudButtons()
    {
        foreach (GameObject openHudButton in openHudButtons)
        {
            openHudButton.SetActive(true);
        }
        openHudButtons.Clear();
    }
}



