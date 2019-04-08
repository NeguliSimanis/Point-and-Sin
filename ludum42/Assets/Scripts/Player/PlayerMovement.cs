using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour {

    #region VARIABLES
    [SerializeField]
    Transform playerLegTransform; // used to check level borders
    private PlayerController playerController;

    [HideInInspector]
    public Vector2 targetPosition;
    /// <summary>
    /// the direction in which the player should move to reach target pos
    /// </summary>
    [HideInInspector]
    public Vector2 dirNormalized;
    [HideInInspector]
    public bool isWalking = false;

    /// <summary>
    /// detected collision with background - player has to stop walking
    /// </summary>
    [HideInInspector]
    public bool isWalkingInObstacle = false;
    /// <summary>
    /// true when movement is locked (e.g. casting spell) and has to remember the last clicked position where you shall move later
    /// </summary>
    [HideInInspector]
    public bool isWaitingToMove = false;

    bool moveUpDisabled = false;
    bool moveLeftDisabled = false;
    bool moveRightDisabled = false;
    bool moveDownDisabled = false;

    bool isOnNorthBorder = false;
    bool isOnSouthBorder = false;
    bool isOnWestBorder = false;
    bool isOnEastBorder = false;
    #endregion

    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isWaitingToMove)
        {
            CheckIfPlayerIsWalking();
        }

        if (isWalking)
        {
            CheckIfPlayerIsWalking();
            MovePlayer();
        }
        //Debug.Log(dirNormalized + " " + Time.time);
    }

    public void GetTargetPositionAndDirection()
    {
        targetPosition = Input.mousePosition;
        targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
        GetDirNormalized(targetPosition);
    }

    /// <summary>
    /// get the direction in which the player should move to reach target pos
    /// </summary>
    void GetDirNormalized(Vector2 sourceVector)
    {
        // calculate direction without any restrictions
        dirNormalized = new Vector2(sourceVector.x - transform.position.x, sourceVector.y - transform.position.y);
        dirNormalized = dirNormalized.normalized;
        //Debug.Log("dirnormal " + dirNormalized);

        // apply movement restrictions in 4 directions - this happens if character has walked onto the edge of the level
        // UP
        if ((moveUpDisabled || isOnNorthBorder) && dirNormalized.y > 0 )
        {
            dirNormalized = new Vector2(dirNormalized.x, 0);
            //Debug.Log("up forbidden ");
        }
        // DOWN
        if ((moveDownDisabled || isOnSouthBorder) && dirNormalized.y < 0 )
        {
            dirNormalized = new Vector2(dirNormalized.x, 0);
            //Debug.Log("down  forbidden ");
        }
        // LEFT
        if ((moveLeftDisabled || isOnWestBorder) && dirNormalized.x < 0 )
        {
            dirNormalized = new Vector2(0, dirNormalized.y);
            //Debug.Log("left forbidden ");
        }
        // RIGHT
        if ((moveRightDisabled || isOnEastBorder) && dirNormalized.x > 0 )
        {
            dirNormalized = new Vector2(0, dirNormalized.y);
            //Debug.Log("right forbidden ");
        }
    }

    /// <summary>
    /// Checks if player is clicking on UI. Used to check whether walking command should be called
    /// </summary>
    /// <returns></returns>
    private bool IsClickingOnUI()
    {
        // clicked on UI - walking not allowedd
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        // not clicking on UI
        return false;
    }

    public bool IsClickingOnWalkableArea()
    {
        // UI - not walkable area
        if (IsClickingOnUI())
            return false;
        return true;
    }

    private void MovePlayer()
    {
        if (playerController.isDeathAnimation)
            return;
        transform.position = new Vector2
            (transform.position.x, transform.position.y) +
            dirNormalized *
            PlayerData.current.moveSpeed *
            Time.deltaTime;
    }


    /// <summary>
    /// Moves the main character if the player has clicked or is holding left mouse
    /// </summary>
    public void ManageMovementInput()
    {
        if (Input.GetMouseButton(0))
        {
            playerController.hasRightClickedRecently = false;
            if (IsClickingOnWalkableArea())
            {
                GetTargetPositionAndDirection();
                CheckIfPlayerIsWalking();
            }
            // playerController.lastClickedTime = Time.time;
        }
        if (Input.GetMouseButtonDown(0))
        {
            // store last clicked time - necessary for item pickup
            playerController.lastClickedTime = Time.time;
        }
        // walking - player clicks
        /* if (Input.GetMouseButtonDown(0))
         {
             // only consider it a mouse press if player has held the key down for minMousePressTime seconds
             if (!isCountingMousePress)
             {
                 isCountingMousePress = true;
                 mousePressStartTime = Time.time + minMousePressTime;
             }
             else if (Time.time > mousePressStartTime)
             {
                 isMousePress = true;
                 playerMovement.CheckIfPlayerIsWalking();
             }
             // store last clicked time
             lastClickedTime = Time.time;
         }*/

        /* // resetting mouse press to move
         if (Input.GetMouseButtonUp(0))
         {
             isMousePress = false;
             isCountingMousePress = false;
         }*/
    }

    private bool CheckOnWhichBorderIsPlayer()
    {
        //Debug.Log("LOOKING " + Time.time);
        RaycastHit2D[] hits = Physics2D.RaycastAll(playerLegTransform.position, Vector2.zero);
        bool topBorderFound = false;
        bool bottomBorderFound = false;
        bool leftBorderFound = false;
        bool rightBorderFound = false;
        for (int arrayID = 0; arrayID < hits.Length; arrayID++)
        {
            //Debug.Log("CHYECK");
            if (hits[arrayID].transform.gameObject.layer == LayerIDs.topBorder)
            {
                topBorderFound = true;
            }
            else if (hits[arrayID].transform.gameObject.layer == LayerIDs.bottomBorder)
            {
                bottomBorderFound = true;
            }
            else if (hits[arrayID].transform.gameObject.layer == LayerIDs.leftBorder)
            {
                leftBorderFound = true;
            }
            else if (hits[arrayID].transform.gameObject.layer == LayerIDs.rightBorder)
            {
                rightBorderFound = true;
            }
        }
        isOnEastBorder = rightBorderFound;
        isOnNorthBorder = topBorderFound;
        isOnSouthBorder = bottomBorderFound;
        isOnWestBorder = leftBorderFound;
        if (bottomBorderFound || leftBorderFound || rightBorderFound || topBorderFound)
        {
            //Debug.Log("smth found");
            return true;
        }
        //Debug.Log("nothing found");
        return false;
    }

    private void DisableMovementAtLevelBorder(RaycastHit2D hit)
    {
        // TOP BORDER
        if (hit.transform.gameObject.layer == LayerIDs.topBorder)
        {
            // attempt to "slide around" the obstacle/border if going from down to up
            if ((targetPosition.x > transform.position.x || targetPosition.x < transform.position.x)
                && targetPosition.y >= transform.position.y)
            {
                AttemptWalkAroundObstacle(Direction.North);
            }
            // just stop walking if the target pos is directly to the north from the border
            else if (targetPosition.y >= transform.position.y)
            {
                moveUpDisabled = true;
                StopWalking();
            }
            else
            {
                moveUpDisabled = false;
            }
        }
        // LEFT BORDDER
        if (hit.transform.gameObject.layer == LayerIDs.leftBorder)
        {
            Debug.Log("left: " + Time.time);
            // attempt to "slide around" the obstacle/border if going from right to left
            if ((targetPosition.y > transform.position.y || targetPosition.y < transform.position.y)
                && !(targetPosition.x >= transform.position.x))
            {
                AttemptWalkAroundObstacle(Direction.West);
            }
            // just stop walking if the target pos is directly to the left from the border
            else if (targetPosition.x <= transform.position.x)
            {
                moveLeftDisabled = true;
                StopWalking();
            }
            else
            {
                moveLeftDisabled = false;
            }
        }
        // BOTTOM BORDER
        if (hit.transform.gameObject.layer == LayerIDs.bottomBorder)
        {
            Debug.Log("bottom: " + Time.time);
            // attempt to "slide around" the obstacle/border if going from UP to DOWN
            if ((targetPosition.x > transform.position.x || targetPosition.x < transform.position.x)
                && targetPosition.y <= transform.position.y)
            {
                AttemptWalkAroundObstacle(Direction.South);
            }
            // just stop walking if the target pos is directly to the south from the border
            else if (targetPosition.y <= transform.position.y)
            {
                moveDownDisabled = true;
                StopWalking();
            }
            else
            {
                moveDownDisabled = false;
            }
        }
        // RIGHT BORDER
        if (hit.transform.gameObject.layer == LayerIDs.rightBorder)
        {

            // attempt to "slide around" the obstacle/border if going from left to right
            if ((targetPosition.y > transform.position.y || targetPosition.y < transform.position.y)
                && !(targetPosition.x <= transform.position.x))
            {
                AttemptWalkAroundObstacle(Direction.East);
            }
            // just stop walking if the target pos is directly to the right from the border
            else if (targetPosition.x >= transform.position.x)
            {
                moveRightDisabled = true;
                StopWalking();
            }
            else
            {
                moveRightDisabled = false;
            }
        }
    }

    private void CheckLevelBorder()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerLegTransform.position, Vector2.zero);
        // RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        //Debug.Log(hit.transform.gameObject.name);
        if (hit.collider != null)
        {
            if (CheckOnWhichBorderIsPlayer())
            {
                DisableMovementAtLevelBorder(hit);
            }
            else
            {
                //Debug.Log("2  " + Time.time);
                ReEnableMovementDirection();
            }        
        }
        else
        {
            //Debug.Log("1  " + Time.time);
            ReEnableMovementDirection();
        }
    }

    /// <summary>
    /// Allow moving in the direction that was previously forbidden if you have walked around the obstacle
    /// </summary>
    private void ReEnableMovementDirection()
    {
            //Debug.Log("IM here " + Time.time);
            GetDirNormalized(targetPosition);
            moveDownDisabled = false;
            moveUpDisabled = false;
            moveLeftDisabled = false;
            moveRightDisabled = false;

            isOnEastBorder = false;
            isOnNorthBorder = false;
            isOnSouthBorder = false;
            isOnWestBorder = false;

    }

    private float GetAbsoluteYDistanceToTarget(bool currentDistance)
    {
        if (currentDistance)
        {
            return Mathf.Abs(targetPosition.y - transform.position.y); 
        }
        else
        {
            return Mathf.Abs(targetPosition.y - (transform.position.y + dirNormalized.y));
        }
    }

    private float GetAbsoluteXDistanceToTarget(bool currentDistance)
    {
        if (currentDistance)
        {
            return Mathf.Abs(targetPosition.x - transform.position.x);
        }
        else
        {
            return Mathf.Abs(targetPosition.x - (transform.position.x + dirNormalized.x));
        }
    }

    /*private void CheckIfPlayerNotOnSouthBorder()
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(playerLegTransform.position, Vector2.zero);
    }*/

    private void AttemptWalkAroundObstacleOnYaxis()
    {
        /// STEP 1 - check what is your absolute distance to target
        float currentYDistanceToTarget = GetAbsoluteYDistanceToTarget(true);
        float futureYDistanceToTarget = GetAbsoluteYDistanceToTarget(false);

        /// STEP 2 - check if moving up or down would bring you closer to target pos
        // NO - stop moving
        if (currentYDistanceToTarget < futureYDistanceToTarget)
        {
            StopWalking();
        }
        // YES
        else if (!moveDownDisabled && !moveUpDisabled)
        {
            // walk up/down BUT
            // don't walk down if on south border
            if (!(isOnSouthBorder && dirNormalized.y < 0)
                // don't walk up if on north border
                && !(isOnNorthBorder && dirNormalized.y > 0))
                dirNormalized = new Vector2(0, dirNormalized.y);
            else
            {
                StopWalking();
            }
        }
    }

    private void AttemptWalkAroundObstacleOnXaxis()
    {
        /// STEP 1 - check what is your absolute distance to target
        float currentYDistanceToTarget = GetAbsoluteXDistanceToTarget(true);
        float futureYDistanceToTarget = GetAbsoluteXDistanceToTarget(false);

        /// STEP 2 - check if moving up or down would bring you closer to target pos
        // NO - stop moving
        if (currentYDistanceToTarget < futureYDistanceToTarget)
        {
            StopWalking();
        }
        // YES - then move
        else if (!moveRightDisabled && !moveLeftDisabled)
        {

            // walk left/right BUT
            // don't walk left if on west border
            if (!(isOnWestBorder && dirNormalized.x < 0)
                // don't walk right if on east border
                && !(isOnEastBorder && dirNormalized.x > 0))
            {
                Debug.Log("IS MOVE");
                dirNormalized = new Vector2(dirNormalized.x, 0);
            }
            else
            {
                StopWalking();
                Debug.Log("SHOULDNT MOVE");
            }
        }
    }

    /// <summary>
    /// Attempt to walk around level border if it brings you closer to target position
    /// </summary>
    /// <param name="relativeObstaclePos">the direction where the obstacle is located relative to player</param>
    private void AttemptWalkAroundObstacle(Direction relativeObstaclePos)
    {
        switch (relativeObstaclePos)
        {
            case Direction.East:
                AttemptWalkAroundObstacleOnYaxis();
                break;
            case Direction.West:
                AttemptWalkAroundObstacleOnYaxis();
                break;
            case Direction.North:
                AttemptWalkAroundObstacleOnXaxis();
                break;
            case Direction.South:
                AttemptWalkAroundObstacleOnXaxis();
                break;
            default:
                Debug.Log("undefinied");
                break;
        }
    }

    /// <summary>
    /// Stop walking
    /// </summary>
    private void StopWalking()
    {
        isWalking = false;
        isWaitingToMove = false;

        dirNormalized = Vector2.zero;
        targetPosition = transform.position;
    }

    public void CheckIfPlayerIsWalking()
    {
        CheckLevelBorder();

        // Player near target position - stop walking
        if (Vector2.Distance(targetPosition, transform.position) <= 0.02f)
        {
            isWalking = false;
            isWaitingToMove = false;
        }
        // movement locked due to melee attack animation
        else if (playerController.isAttacking)
        {
            isWalking = false;
            // make sure that you only register movement commands since after attack command
            if (!playerController.hasRightClickedRecently)
                isWaitingToMove = true;
        }
        // movement locked due to fireball animation
        else if (playerController.isCastingSpell)
        {
            isWalking = false;
            // make sure that you only register movement commands since after attack command
            if (!playerController.hasRightClickedRecently)
                isWaitingToMove = true;
        }
        /*else if (isWalkingInObstacle)
        {
            isWalking = false;
            isWaitingToMove = false;
            isWalkingInObstacle = false;
        }*/
        else if (playerController.isClickingOnUI)
        {
            isWalking = false;
            isWaitingToMove = false;
            playerController.isClickingOnUI = false;
        }
        else
        {
            isWalking = true;
        }
    }

}
