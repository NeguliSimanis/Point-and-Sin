using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    bool isLeftMouseDown = false;
    bool isRightMouseDown = false;
    private Animator cursorAnimator;
    string clipName;
    string clickClipName = "cursor_click_anim";
    AnimatorClipInfo[] currentClipInfo;

    private void Start()
    {
        Cursor.visible = false;
        cursorAnimator = gameObject.GetComponent<Animator>();
    }


    private void ManageMouseInput()
    {
        Cursor.visible = false;

        // force end the mouse click animation and go to default state
        if (clipName == "clickClipName")
        {
            cursorAnimator.SetTrigger("forceEndClick");
        }

        // animate cursor on left click
        cursorAnimator.SetBool("isMouseDown", isLeftMouseDown);

        // player is casting spell AND right clicking - animate cursor
        if (playerController.isCastingSpell && isRightMouseDown)
        {
            cursorAnimator.SetBool("isRightMouseDown", isRightMouseDown);
        }
        // player is not right clicking - don't animate cursor
        else if (!isRightMouseDown)
        {
            cursorAnimator.SetBool("isRightMouseDown", isRightMouseDown);
        }
    }

    /// <summary>
    /// Get animator info so that we can check a particular animation clip is playing
    /// </summary>
    private void GetAnimatorInfo()
    {
        currentClipInfo = this.cursorAnimator.GetCurrentAnimatorClipInfo(0);
        clipName = currentClipInfo[0].clip.name;
    }

    private void ManageLeftClick()
    {
        if (Input.GetMouseButton(0))
        {
            isLeftMouseDown = true;
            GetAnimatorInfo();
        }
        else
        {
            isLeftMouseDown = false;
        }
    }

    private void ManageSpellClick()
    {
        if (playerController.isCastingSpell)
        {
            isRightMouseDown = true;
            GetAnimatorInfo();
        }
        else
        {
            isRightMouseDown = false;
        }
    }

    private void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
        ManageLeftClick();
        ManageSpellClick();
        ManageMouseInput();
    }
}
