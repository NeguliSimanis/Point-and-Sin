using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    EnemyController.EnemyType type;
    [SerializeField] AudioClip deathSFX;
    float deathSFXVolume = 1f;

    [SerializeField] AudioClip[] attackSFXS;
    float attackSFXVolume = 0.5f;
    AudioSource enemyAudioSource;

    private void Start()
    {
        if (type == EnemyController.EnemyType.Demon)
        {
            enemyAudioSource = gameObject.GetComponent<AudioSource>();
        }
    }

    public void PlaySFX(SFXType type)
    {
        if (type == SFXType.Death)
        {
            CallEnemyDeathSFX();
        }
        else if (type == SFXType.DemonMeleeAttack1 || type == SFXType.DemonMeleeAttack2)
        {
            PlayEnemyMeleeAttackSFX(type);
        }
        
    }

    private void PlayEnemyMeleeAttackSFX(SFXType attackType)
    {
        if (attackType == SFXType.DemonMeleeAttack1)
        {
            enemyAudioSource.PlayOneShot(attackSFXS[0], attackSFXVolume);
        }
        else if (attackType == SFXType.DemonMeleeAttack2)
        {
            enemyAudioSource.PlayOneShot(attackSFXS[1], attackSFXVolume);
        }
    }

    /// <summary>
    /// As of now, only called from demon death animations
    /// </summary>
    private void CallEnemyDeathSFX()
    {
        PlayEnemyDeathSFX(type, deathSFX, deathSFXVolume, enemyAudioSource, true);
    }

    public void PlayEnemyDeathSFX(
        EnemyController.EnemyType type,
        AudioClip deathSFX,
        float deathSFXVolume,
        AudioSource enemyAudioSource,
        bool isCalledByAnimation = false)
    {
        if (type != EnemyController.EnemyType.Demon ||
            (type == EnemyController.EnemyType.Demon && isCalledByAnimation))
        enemyAudioSource.PlayOneShot(deathSFX, deathSFXVolume);
    }
}
