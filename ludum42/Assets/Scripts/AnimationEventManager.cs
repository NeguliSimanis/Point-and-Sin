using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains methods that are called from animation frames
/// </summary>
public class AnimationEventManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] characterShadows;
    int previousShadowID = -1;
    public void SetCharacterShadows(int activeShadowID)
    {
        if (characterShadows.Length == 0)
        {
            Debug.Log("char shadows not set!");
            return;
        }

        characterShadows[activeShadowID].SetActive(true);
        if (previousShadowID != -1 && previousShadowID != activeShadowID)
        {
            characterShadows[previousShadowID].SetActive(false);
        }
        previousShadowID = activeShadowID;
    }

    [SerializeField]
    GameObject[] gameObjectsToActivate;
    public void ActivateGameObjects(int objectID)
    {
        gameObjectsToActivate[objectID].SetActive(true);
    }

    [SerializeField]
    AudioManager audioManager;
    public void PlaySFX(SFXType type)
    {
        audioManager.PlaySFX(type);
    }

    public void DealMeleeDamageToPlayer()
    {
        EnemyController enemyController;
        enemyController = transform.parent.gameObject.GetComponent<EnemyController>();
        enemyController.MeleeDamagePlayer();
    }
}
