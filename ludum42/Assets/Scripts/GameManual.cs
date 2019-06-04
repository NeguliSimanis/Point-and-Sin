using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManual : MonoBehaviour
{
    [SerializeField]
    Text helpText;

    string gameplayHelpText = "Kill enemies to level up. With each new level, you gain 2 sin points, which can be spent to make you stronger.\n\nEnemies drop items. Equip items to get stronger. Eat items to recover life or mana.\n\nUnlock brutal mode by defeating the boss and finding the vial of virgin blood.";
    string controlsHelpText = "Right click  -  use active ability\nLeft click  -  walk, pick up items\nMouse wheel  –  switch active ability\nC  -  open character panel\nI  -  open inventory\nEsc  -  pause game";
    string sinsHelpText = "Wrath - increases melee damage, spell damage; increases spell cost\n\nPride - increases health and mana\n\nLust - increases mana regeneration, melee critical chance, reduces spell cooldown";

    public void ShowGameplayText()
    {
        helpText.text = gameplayHelpText;
    }

    public void ShowControlsText()
    {
        helpText.text = controlsHelpText;
    }

    public void ShowSinsText()
    {
        helpText.text = sinsHelpText;
    }
}
