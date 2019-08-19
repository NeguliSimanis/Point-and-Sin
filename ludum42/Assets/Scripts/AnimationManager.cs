using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    #region skipping animations
    bool hasSkipped = false;
    bool skipAnimationOnAnyKey = true;

    [Header("Skipping Cascade")]
    [SerializeField]
    bool cascadeAnimationSkip = false;
    [SerializeField]
    AnimationManager [] otherAnimationsToSkip;

    [Header("Skipping Scripted Fade-in")]
    [SerializeField]
    bool skipScriptedFadeIn = false;
    [SerializeField]
    FadeIn[] fadeInsToSkip;

    [Header("Skipped Events")]
    [SerializeField]
    bool callSkippedAnimationEvent = false;
    [SerializeField]
    int[] skippedAnimationEventIDs;
    #endregion

    Animator animator;
    AnimationEventManager animationEventManager;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (hasSkipped)
            return;
        if (Input.anyKey && skipAnimationOnAnyKey)
        {
            SkipAnimation();    
        }
    }

    public void SkipAnimation()
    {
        hasSkipped = true;
        animator.SetTrigger("skip");
        if (callSkippedAnimationEvent)
        {
            CallSkippedAnimationEvent();
        }
        if (cascadeAnimationSkip)
        {
            SkipOtherAnimations();
        }
        if (skipScriptedFadeIn)
        {
            SkipScriptedFadeIn();
        }
    }

    private void SkipScriptedFadeIn()
    {
        for (int i = 0; i < fadeInsToSkip.Length; i++)
        {
            fadeInsToSkip[i].SkipFade(true,true);
        }
    }

    private void SkipOtherAnimations()
    {
        for (int i = 0; i < otherAnimationsToSkip.Length; i++)
        {
            otherAnimationsToSkip[i].SkipAnimation();
        }
    }

    private void CallSkippedAnimationEvent()
    {
        animationEventManager = gameObject.GetComponent<AnimationEventManager>();
        Debug.Log("begnning skip");
        for (int i = 0; i < skippedAnimationEventIDs.Length; i++)
        {
            Debug.Log("calling " + i);
            animationEventManager.ActivateGameObjects(i);
        }
    }
}
