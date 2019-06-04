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
    private Direction moveDirection;
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

    [Header("DEBUG")]
    [SerializeField]
    GameObject playerLineMarker;
    [SerializeField]
    GameObject playerDestinationMarker;
    #endregion


    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        transform.position = PixelPerfectClamp(transform.position);
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

    /// <summary>
    /// Determines current movement direction (East, West, etc.) from dirNormalized vector
    /// </summary>
    private void GetMoveDirection()
    {
        if (dirNormalized == null)
            return;

        float y = dirNormalized.y;
        float x = dirNormalized.x;
        bool directionFound = false;
        //Debug.Log("x: " + x + ". y: " + y + ". Time: " + Time.time);

        #region NOTES
        /*
        *                 (0x; 1y)
        *         \     ↑     / 
        *          \ North   /  
        *           \   |   /   Not North
        *  Not North \  |  / 
        *             \ | /       
        *         120° \|/  60°  
        * - - - - - - - | - - - - - - - - → (1x; 0y)
        *               | (0x; 0y)         
        *     
        *     
        *   Relation between X and Y values is described by cotangent (cot)              
        *   
        *   X values for sector between 90° and 60°:
        *      1) cot(30°) = y/x = sqrt(3)           
        *      2) x(30°) < y / sqrt(3)  
        *      3) sqrt(3) = ~1.73
        *      4) y / 1.73 = y * (100/173) = y * ~0.58
        *      5) x < 0.58y
        *   
        *   X values for sector between 90° and 30°:
        *      1) cot(60°) = y/x = sqrt(3)/3          
        *      2) x = 3/sqrt(3) * y  
        *      3) 3/sqrt(3) = ~1.73
        *      4) x > ~1.73
        *   
        *    X values for sector between 120° and 90°:
        *      1) cot(30°) = y/x = sqrt(3)           
        *      2) x(30°) > y / sqrt(3)  
        *      3) sqrt(3) = ~1.73
        *      4) y / 1.73 = y * (100/173) = y * ~0.58
        *      5) -x > -0.58y
        *      
        *   Y values for sector between 30° and -30°
        *       1) cot(30°) = y/x = sqrt(3)  
        *       2) y = x * sqrt(3)
        *       3) sqrt(3) = ~1.73
        *       4) x / 1.73 = x * (100/173) = x * ~0.58
        *       4) less than 30° => y < 0.58x
        *       5) more than -30° = -y > 0.58x 
        *   
        *   000 WRONG calculations BELOW 000
        *   X values for sector between 0° and 60°:
        *      1) cot(60°) = y/x = sqrt(3)/3              
        *      2) x = 3/sqrt(3) * y
        *      3) 3/sqrt(3) = ~1.73
        *      4) x > 0 && x < 1.73 y
        *      
        *   X values for sector between 120° and 0°:
        *      1) cot(120°) = y/x = -sqrt(3)/3              
        *      2) x = -3/sqrt(3) * y
        *      3) 3/sqrt(3) = ~ -1.73
        *      4) x < 0 && x > -1.73 y
        *      
        *     Final X values for pure NORTH (between 120° and 60°): 
        *         -1.73y < x < 1.73 y 
        *         
        *     X values for sector NORTH-EAST (between 60° and 30°):
        *          1) cot(30°) = y/x = sqrt(3)           
        *          2) x(30°) = y / sqrt(3)  
        *          3) sqrt(3) = ~1.73
        *          4) y / 1.73 = y * (100/173) = y * ~0.58
        *          5) x(30°) = 0.58y //1.73 y > x > 0.58y
        *          
        *      X values for sector NORTH-WEST (between 150° and 120°):
        *          1) cot(150°) = y/x = -sqrt(3)           
        *          2) x(150°) = y / -sqrt(3)  
        *          3) sqrt(3) = ~-1.73
        *          4) y / -1.73 = y * -(100/173) = y * -~0.58
        *          5) x(150°) = -0.58*y //-0.58*y < x < -1.73*y
        */
        #endregion
        // NORTH
        if (y > 0)
        {
            #region pure NORTH
            /* // (between 150° and 60°) 
            if (x < 0.58 * y && // true if angle more than 60°
                -x > -0.58 * y) // true if angle less than 150°
            {
                moveDirection = Direction.North;
            }*/
            #endregion
            #region NORTH-EAST
            // between 90° and 30° 
            if (x < 1.73 * y && // true if angle more than 30°
                x > 0)          // true if angle less than 90°
            {
                //Debug.Log("moving ne " + Time.time);
                moveDirection = Direction.NorthEast;
                directionFound = true;
            }
            #endregion
            #region NORTH-WEST
            // between 150° and 90°
            else if (x < 0 &&           // true if angle more than 90°
                    -x > -1.73 * y)     // true if angle less than 150°
            {
                //Debug.Log("moving nw " + Time.time);
                moveDirection = Direction.NorthWest; 
                directionFound = true;
            }
            #endregion
        }
        // SOUTH
        else if (y < 0)
        {
            #region SOUTH-WEST
            // between 210° and 270°
            if (x < 0 &&            // true if angle less than 270°
                x > 1.73 * y)     // true if angle more than 210°
            {
                //Debug.Log("moving sw " + Time.time);
                moveDirection = Direction.SouthWest;
                directionFound = true;
            }
            #endregion
            #region SOUTH-EAST
            // between 270° and 330°
            if (x > 0 &&            // true if angle more than 270°
                -x > 1.73 * y)       // true if angle less than 330°
            {
               // Debug.Log("moving se " + Time.time);
                moveDirection = Direction.SouthEast;
                directionFound = true;
            }
            #endregion
        }
        // WEST
        if (x > 0)
        {
            // between 30° and -30°
            if (y < 0.58 * x &&     // angle less than 30° 
                -y > -0.58 * x)       // angle more than -30°
            //if (!directionFound)
            {
                //Debug.Log(y + "x: " + x + " moving w " + Time.time);
                moveDirection = Direction.West;
            }
        }
        // EAST
        else if (x < 0)
        {
            // between 150° and 210°
            if (y < -0.58 * x &&     // angle more than 150° 
                y > 0.58 * x)       // angle less than -210°
            {
                //Debug.Log("moving e " + Time.time);
                moveDirection = Direction.East;
            }

        }


        //Debug.Log("Moving " + moveDirection + ". Time: " + Time.time);
    }

    public void GetTargetPositionAndDirection()
    {
        targetPosition = Input.mousePosition;
        targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
        Instantiate(playerDestinationMarker, targetPosition, Quaternion.identity);
        GetDirNormalized(targetPosition);
    }

    /// <summary>
    /// get the direction in which the player should move to reach target pos
    /// </summary>
    void GetDirNormalized(Vector2 sourceVector)
    {
        // calculate direction without any restrictions
        dirNormalized = new Vector2(sourceVector.x - transform.position.x, sourceVector.y - transform.position.y);
        dirNormalized = PixelPerfectClamp(dirNormalized);
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
        Debug.Log(targetPosition + " " + Time.time);
        GetMoveDirection();

        Vector2 moveVector = dirNormalized*
            PlayerData.current.moveSpeed *
            Time.deltaTime;

        moveVector = PixelPerfectClamp(moveVector);

        transform.position = new Vector2
            (transform.position.x, transform.position.y) + moveVector;

        Instantiate(playerLineMarker, transform.position, Quaternion.identity);
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
            //Debug.Log("left: " + Time.time);
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
            //Debug.Log("bottom: " + Time.time);
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
                //Debug.Log("IS MOVE");
                dirNormalized = new Vector2(dirNormalized.x, 0);
            }
            else
            {
                StopWalking();
                //Debug.Log("SHOULDNT MOVE");
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

    /// <summary>
    /// Code taken from https://www.youtube.com/watch?v=OBulUgXe7rA
    /// </summary>
    /// <param name="moveVector"></param>
    /// <param name="pixelsPerUnit"></param>
    /// <returns></returns>
    private Vector2 PixelPerfectClamp(Vector2 moveVector)
    {
        Vector2 vectorInPixels = new Vector2(
            Mathf.RoundToInt(moveVector.x * PlayerData.current.pixelsPerUnit),
            Mathf.RoundToInt(moveVector.y * PlayerData.current.pixelsPerUnit));

        Vector2 clampedVector = vectorInPixels / PlayerData.current.pixelsPerUnit;


        // targetPosition = new Vector2(targetPositionWithoutOffset.x, targetPositionWithoutOffset.y + legYOffset);

        // WEST
        // Distance more than 1 pixel, but rounded movement vector is less than 1 pixel
        if (clampedVector.x == 0 && targetPosition.x > transform.position.x + 1 / PlayerData.current.pixelsPerUnit)
        {
            clampedVector.x = 1 / PlayerData.current.pixelsPerUnit;
        }

        // EAST
        // Distance more than 1 pixel, but rounded movement vector is less than 1 pixel
        if (clampedVector.x == 0 && targetPosition.x < transform.position.x - 1 / PlayerData.current.pixelsPerUnit)
        {
            clampedVector.x = -1 / PlayerData.current.pixelsPerUnit;
        }

        // NORTH
        // Distance more than 1 pixel, but rounded movement vector is less than 1 pixel

        if (clampedVector.y == 0 && targetPosition.y > transform.position.y + 1 / PlayerData.current.pixelsPerUnit)
        {
            clampedVector.y = 1 / PlayerData.current.pixelsPerUnit;
        }

        // SOUTH
        // Distance more than 1 pixel, but rounded movement vector is less than 1 pixel
        if (clampedVector.y == 0 && targetPosition.y < transform.position.y - 1 / PlayerData.current.pixelsPerUnit)
        {
            clampedVector.y = -1 / PlayerData.current.pixelsPerUnit;
        }



        return clampedVector;
    }

}
