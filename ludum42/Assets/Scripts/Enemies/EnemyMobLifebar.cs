using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMobLifebar : MonoBehaviour
{
    [SerializeField] GameObject enemyLifebar;
    [SerializeField] Image lifebarImage;
    [SerializeField] PlayerController playerController;
    
    bool isLifebarActive = false;
    public EnemyController currentEnemyController;

    float lifebarDisappearTime;
    float lifebarDuration = 3f;

	/// <summary>
    /// Activates/deactivates enemy lifebar object when you hover over enemy
    /// </summary>
	private void ToggleLifebar(bool activateLifebar)
    {
        if (!activateLifebar)
        {
            enemyLifebar.SetActive(false);
        }
        else
        {
            enemyLifebar.SetActive(true);
        }
    }
	
    private void UpdateLifebarFillAmount()
    {
        lifebarImage.fillAmount = (currentEnemyController.currentHP * 1f) / currentEnemyController.maxHP;
    }

    private void HideLifebar()
    {
        ToggleLifebar(false);
        isLifebarActive = false;
    }

	void Update ()
    {
		if (playerController.isMouseOverEnemy)
        {
            ToggleLifebar(true);
            isLifebarActive = true;
            lifebarDisappearTime = Time.time + lifebarDuration;
        }
        /*else if (isLifebarActive)
        {
            ToggleLifebar(false);
            isLifebarActive = false;
        }*/
        if (isLifebarActive)
        {
            currentEnemyController = playerController.lastHoveredEnemy;
            UpdateLifebarFillAmount();
            if (currentEnemyController.currentHP <= 0)
            {
                HideLifebar();
            }

            if (Time.time > lifebarDisappearTime)
            {
                HideLifebar();
            }
        }
	}
}
