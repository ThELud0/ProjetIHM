using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed;
    public float jumpSpeed;
    public float climbSpeed;
    public Transform playerLowerLeftCornerCheck;
    public Transform playerLowerRightCornerCheck;
    public Transform playerUpperLeftCornerCheck;
    public Transform playerUpperRightCornerCheck;
    public LayerMask groundLayer;
    public LayerMask climbLayer;
    public LayerMask wallLayer;
    public float groundCheckRadius;
    public float climbCheckRadius;
    public float wallCheckRadius;
    public int maxJumpAmount;
    public float sprintSpeedCoef;
    public float doubleTapTimeThreshold;
    public float normalGravityScale;
    public float jumpingTimeWindow;
    public float jumpAnimationTime;
    public float jumpAnimationSize;

    public AudioClip jumpSoundClip;
    public AudioClip dashSoundClip;

    public int maxHealth;
    public float invincibilityDuration;
    

    private Gamepad manette;

    //Keyboard binds
    private KeyCode shiftKeyboardInput = KeyCode.LeftControl;
    private KeyCode shiftAltKeyboardInput = KeyCode.RightControl;
    private KeyCode leftSprintKeyboardInput = KeyCode.A; //Unity se base sur des claviers QWERTY...
    private KeyCode rightSprintKeyboardInput = KeyCode.D;


    private float tempStopGroundCheckTimer = 0.05f;
    private float tempStopClimbCheckTimer = 0.1f;
    private float jumpTimestamp = 0f;
    private int jumpCounter;
    private float noGravityScale = 0f;

    private float leftLastTapTime = 0f;
    private float rightLastTapTime = 0f;
    private bool sprinting;

    private bool isGrounded;

    private bool isClimbing;
    private bool jumpRefreshed;
    private bool wallJumpRefreshed;
    private bool isTouchingWall;
    private bool canStillJump;
    private bool trailLocked;
    private float canStillJumpTimestamp = 0f;

    private Rigidbody2D player;
    private SpriteRenderer playerSprite;
    private Vector2 direction;
    private float previousMoveDirection = 0f;
    private float previousMoveDirectionTimestamp = 0f;
    private float previousMoveDirectionTimestampOffset = 0.05f;

    private Vector3 respawnPoint;
    private Vector3 initialScale;
    private Vector3 currentScale;
    private Color defaultColor;
    private float invincibilityStartTime = 0f;
    private float flickerTimeStamp = 0f;
    private bool flickering = false;
    public float flickerOnDamageTime;
    public float flickerOnDamageSpeed;


    private float CurrentDashTimer;
    public float DashTime = 0.3f;
    private bool IsDashing = false;
    private Vector2 DashDirection;
    public float dashSpeed = 30f;
    public LayerMask collisionLayer;//#TODO_N there is still problems with collision I should deal with, maybe in the matrix ?
    private bool hasDashed = false;
    TrailRenderer trail;

    /* -------------------------------------------------- BEGINNING OF START METHOD -------------------------------------------------- */
    void Start()
    {
        // Subscribe to the device change event
        InputSystem.onDeviceChange += OnDeviceChange;


        player = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        manette = Gamepad.current;
        isClimbing = false;
        jumpRefreshed = false;

        respawnPoint = transform.position;
        initialScale = transform.localScale;
        currentScale = initialScale;

        trail = GetComponent<TrailRenderer>();
        trail.emitting = false;
        trailLocked = false;
        ChangeColor(Color.white);

        if (FeedbackAnimationParameters.health <= 0)
        {
            FeedbackAnimationParameters.health = maxHealth;
        }
    }

    /* -------------------------------------------------- END OF START METHOD -------------------------------------------------- */




    /* -------------------------------------------------- BEGINNING OF UPDATE METHOD -------------------------------------------------- */

    // Update is called once per frame
    void Update()
    {
        // Check if there is ground under the player
        if ((Physics2D.OverlapCircle(playerLowerLeftCornerCheck.position, groundCheckRadius, groundLayer)) || 
            (Physics2D.OverlapCircle(playerLowerRightCornerCheck.position, groundCheckRadius, groundLayer)))
            isGrounded = true;
        else
            isGrounded = false;

        // Reset jump counter and ability to dash if player is on the ground
        if ((isGrounded) && (Time.time > jumpTimestamp + tempStopGroundCheckTimer)) //Stop checking for ground for a short time after initiating a jump to not reset the counter right away
        {
            jumpCounter = maxJumpAmount;
            hasDashed = false;
        }

        // Player is not submitted to gravity if climbing
        if (!isClimbing)
            player.gravityScale = normalGravityScale;
        else if (isClimbing)
            player.gravityScale = noGravityScale;



        HorizontalMovement();

        CheckIfTouchingWall();
        if (isTouchingWall)
        {
            if (!wallJumpRefreshed)
            {
                Debug.Log("wallJumpRefreshed");
                jumpCounter = maxJumpAmount;
                wallJumpRefreshed = true;
            } 
        }
        else if (!isTouchingWall)
            wallJumpRefreshed = false;
        


        // check if player is near climbable surface
        if (CheckAndReturnIfPlayerCanClimb())
        {
            //check if player gives climbing input and set isClimbing bool accordingly
            CheckClimbInputAndSet();
            if (isClimbing  && (Time.time > jumpTimestamp + tempStopClimbCheckTimer))
            {
                // get player keyboard input for y-axis movement
                float verticalMove = Input.GetAxis("Vertical") * climbSpeed;
                if ((verticalMove == 0) && (manette != null)) //if no keyboard input check for gamepad input
                {
                    direction = manette.dpad.ReadValue();
                    verticalMove = direction.y * climbSpeed;
                }
                // apply move speed to player velocity
                player.velocity = new Vector2(player.velocity.x, verticalMove);
            }
        }
        else
        {
            NotClimbingOrStopped(); //player cannot be climbing if not near climbable surface
        }
        ClimbAnimation();


        CheckAndExecuteJump();

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        if (((moveX == 0)&&(moveY == 0)) && (manette != null)) //if no keyboard input check for gamepad input
        {
            direction = manette.dpad.ReadValue();
            moveX = direction.x;
            moveY = direction.y;
        }
        //#TODO_N get how we deal with gamepad/keyboard
        if ( ((manette != null && manette.buttonEast.wasPressedThisFrame) || Input.GetKeyDown(KeyCode.LeftShift) ) && !IsDashing && (moveX != 0 || moveY != 0) && !hasDashed)
        {
            SoundFXManager.instance.PlaySoundFXClip(dashSoundClip, transform, 1f);
            IsDashing = true;
            CurrentDashTimer = DashTime;
            player.velocity = Vector2.zero;

            DashDirection = new Vector2(moveX, moveY).normalized;
            SetTrailEmissionState(true);
        }

        if (IsDashing)
            Dash();

        if (flickering)
            Flicker();
    }

    /* -------------------------------------------------- END OF UPDATE METHOD -------------------------------------------------- */




    /* -------------------------------------------------- BEGINNING OF DASH METHODS -------------------------------------------------- */
    private void Dash()
    {
        hasDashed = true;
        
        player.velocity = DashDirection * dashSpeed;

        CurrentDashTimer -= Time.deltaTime;

        if (CurrentDashTimer <= 0)
        {
            IsDashing = false;
            player.velocity = Vector2.zero;

            if (!sprinting)
                SetTrailEmissionState(false);
        }
    }


    /* -------------------------------------------------- END OF DASH METHODS -------------------------------------------------- */




    /* -------------------------------------------------- BEGINNING OF JUMP METHODS -------------------------------------------------- */


    private void CheckAndExecuteJump()
    {
        if (isGrounded || isClimbing || isTouchingWall)
        {
            canStillJumpTimestamp = Time.time;
        }
        if (Time.time < canStillJumpTimestamp + jumpingTimeWindow)
        {
            canStillJump = true;
        }
        else
            canStillJump = false;

        if ((Input.GetButtonDown("Jump")) && (jumpCounter == maxJumpAmount) && canStillJump)
            PlayerJumpUp();
        
        else if ((Input.GetButtonDown("Jump")) && (jumpCounter > 0) && (jumpCounter < maxJumpAmount)) //check for jump input and make player jump if there are jumps remaining in jump counter and player is in air
        {
            PlayerJumpUp();

        }
        else if (manette != null)
        {
            if ((manette.buttonSouth.wasPressedThisFrame) && (jumpCounter == maxJumpAmount) && canStillJump)
                PlayerJumpUp();
            else if ((manette.buttonSouth.wasPressedThisFrame) && (jumpCounter > 0) && (jumpCounter < maxJumpAmount))             // check gamepad jump input
                PlayerJumpUp();
            
        }
    }

    /// <summary>
    /// Make player jump up
    /// </summary>
    private void PlayerJumpUp()
    {
        PlayerJumpAnimation();
        SoundFXManager.instance.PlaySoundFXClip(jumpSoundClip, transform, 1f);
        player.velocity = new Vector2(player.velocity.x, jumpSpeed);
        jumpCounter--;
        jumpTimestamp = Time.time;
    }




    /* -------------------------------------------------- END OF JUMP METHODS -------------------------------------------------- */



    /* -------------------------------------------------- BEGINNING OF MOVEMENT METHODS -------------------------------------------------- */

    private void HorizontalMovement()
    {
        // get player keyboard input for x-axis movement
        float move = Input.GetAxis("Horizontal") * moveSpeed;
        if ((move == 0) && (manette != null)) //if no keyboard input check for gamepad input
        {
            direction = manette.dpad.ReadValue();
            move = direction.x * moveSpeed;
        }
        // check for sprinting input and modify move speed accordingly
        move = CheckAndApplyPlayerHorizontalSprint(move);
        // apply move speed to player velocity
        player.velocity = new Vector2(move, player.velocity.y);
    }




    /// <summary>
    /// Check if player is sprinting and if so, multiply their move speed by sprinting factor
    /// </summary>
    /// <param name="move">move is a float corresponding to the x-axis velovity for the player</param>
    /// <returns></returns>
    private float CheckAndApplyPlayerHorizontalSprint(float move)
    { 


        if (manette != null)
        {
            if ((manette.leftShoulder.isPressed) && isGrounded)
                Sprint();
            else if (manette.leftShoulder.wasReleasedThisFrame)
                EndSprint();
        }

        // Check for a double tap on Q key
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(leftSprintKeyboardInput)) //Unity adapté à clavier QWERTY... donc Q = A :')
        {
            if ((Time.time - leftLastTapTime <= doubleTapTimeThreshold) && isGrounded)
                Sprint();
            else
                EndSprint();
            leftLastTapTime = Time.time;
        }
        // Check for a double tap on D key
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(rightSprintKeyboardInput))
        {
            if ((Time.time - rightLastTapTime <= doubleTapTimeThreshold)&& isGrounded)
                Sprint();
            else
                EndSprint();
            
            rightLastTapTime = Time.time;
        }

        //stop sprinting if there is a sudden change in direction
        bool samesign = (move < 0) == (previousMoveDirection < 0);
        if (!samesign)
            EndSprint();


        if (sprinting)
            move *= sprintSpeedCoef;
        
        //record current direction as previous move direction for future iteration
        if (Time.time > previousMoveDirectionTimestamp + previousMoveDirectionTimestampOffset)
        {
            previousMoveDirection = move;
            previousMoveDirectionTimestamp = Time.time;
        }


        return move;
    }

    private void Sprint()
    {
        sprinting = true;
        SetTrailEmissionState(true);
    }

    private void EndSprint()
    {
        sprinting = false;
        if (!IsDashing)
            SetTrailEmissionState(false);
    }

    /* -------------------------------------------------- END OF MOVEMENT METHODS -------------------------------------------------- */





    /* -------------------------------------------------- BEGINNING OF CLIMBING METHODS -------------------------------------------------- */

    /// <summary>
    /// Checks if a climbing input was given by the player and calls climbing methods if so
    /// </summary>
    private void CheckClimbInputAndSet()
    {
        if ((Input.GetKeyUp(shiftKeyboardInput)) || (Input.GetKeyUp(shiftAltKeyboardInput)) )
        {
            NotClimbingOrStopped();
        }
        if ((Input.GetKey(shiftKeyboardInput)) || (Input.GetKey(shiftAltKeyboardInput)))
        {
            StartClimbing();
        }

        if (manette != null)
        {
            if (manette.rightShoulder.isPressed)
            {
                StartClimbing();
            }
            else if (manette.rightShoulder.wasReleasedThisFrame)
            {
                NotClimbingOrStopped();
            }
        }
        
    }

    /// <summary>
    /// player is not climbing or stopped climbing
    /// </summary>
    private void NotClimbingOrStopped()
    {

        isClimbing = false;
        jumpRefreshed = false;

    }

    /// <summary>
    /// sets isClimbing to true and refreshes jumpCounter once per beginning of climb
    /// </summary>
    private void StartClimbing()
    {
        if (!jumpRefreshed)
        {
            jumpCounter = maxJumpAmount;
            jumpRefreshed = true;
        }
        sprinting = false;
        isClimbing = true;
        
    }

    /// <summary>
    /// Check if player is near climb-able objects
    /// </summary>
    private bool CheckAndReturnIfPlayerCanClimb()
    {
        //if player is near climbable surface on left/right side
        if ((Physics2D.OverlapCircle(playerLowerLeftCornerCheck.position, climbCheckRadius, climbLayer)) || 
            (Physics2D.OverlapCircle(playerLowerRightCornerCheck.position, climbCheckRadius, climbLayer)) ||
            (Physics2D.OverlapCircle(playerUpperLeftCornerCheck.position, climbCheckRadius, climbLayer)) ||
            (Physics2D.OverlapCircle(playerUpperRightCornerCheck.position, climbCheckRadius, climbLayer)))
            return true;
        else
            return false;
    }




    /* -------------------------------------------------- END OF CLIMBING METHODS -------------------------------------------------- */




    /* -------------------------------------------------- BEGINNING OF FEEDBACK/ANIMATION METHODS -------------------------------------------------- */

    private void ClimbAnimation()
    {
        if (FeedbackAnimationParameters.playerClimbAnimationActivated)
        {
            if (isClimbing)
            {
                ChangeColor(Color.green);
            }
            else if (!flickering)
            {
                ChangeColor(Color.white);
            }
        }
            
    }

    private void PlayerJumpAnimation()
    {
        if (FeedbackAnimationParameters.playerJumpAnimationActivated)
        {
            DecreaseSize();
            Invoke("RestoreSize", jumpAnimationTime);
        }

    }

    private void SetTrailEmissionState(bool state)
    {
        if (FeedbackAnimationParameters.playerTrailAnimationActivated && !trailLocked)
            trail.emitting = state;
        else
            trail.emitting = false;
    }


    /* -------------------------------------------------- END OF FEEDBACK/ANIMATION METHODS -------------------------------------------------- */





    /* -------------------------------------------------- BEGINNING OF MISCELLEANOUS METHODS -------------------------------------------------- */

    private void CheckIfTouchingWall()
    {
        if ((Physics2D.OverlapCircle(playerLowerLeftCornerCheck.position, wallCheckRadius, wallLayer)) ||
    (Physics2D.OverlapCircle(playerLowerRightCornerCheck.position, wallCheckRadius, wallLayer)) ||
    (Physics2D.OverlapCircle(playerUpperLeftCornerCheck.position, wallCheckRadius, wallLayer)) ||
    (Physics2D.OverlapCircle(playerUpperRightCornerCheck.position, wallCheckRadius, wallLayer)))
        {
            isTouchingWall = true;
            sprinting = false;
        }
            
        else
            isTouchingWall = false;
    }

    private void InitializePlayerAtCheckPointAfterDeath()
    {
        transform.position = respawnPoint;
        trail.time = 0.23f;
    }

    private void ResetPlayerState()
    {
        player.velocity = new Vector2(0, 0);
        jumpRefreshed = false;
        wallJumpRefreshed = false;
        IsDashing = false;
        isClimbing = false;
        hasDashed = false;
        sprinting = false;
        isClimbing = false;
        isGrounded = false;
        isTouchingWall = false;
        TrailTemporaryLock();
        trail.time = 0.01f;
        trail.emitting = false;
        ChangeColor(Color.white);
    }

    private void DecreaseSize()
    {
        transform.localScale = new Vector3(currentScale.x * jumpAnimationSize, currentScale.y * jumpAnimationSize, currentScale.z * jumpAnimationSize);
        wallCheckRadius += 0.1f;
        groundCheckRadius += 0.1f;
        climbCheckRadius += 0.05f;
    }

    private void RestoreSize()
    {
        transform.localScale = currentScale;
        wallCheckRadius -= 0.1f;
        groundCheckRadius -= 0.1f;
        climbCheckRadius -= 0.05f;
    }

    private void ChangeColor(Color color)
    {
        playerSprite.color = color;
    }

    private void TrailTemporaryLock()
    {
        trailLocked = true;
        Invoke("TrailUnlock",0.1f);
    }

    private void TrailUnlock()
    {
        trailLocked = false;
    }

    private void Flicker()
    {
        defaultColor = playerSprite.color;

        if (Time.time - flickerTimeStamp >= 1f / flickerOnDamageSpeed)
        {
            Debug.Log(defaultColor.a);


            flickerTimeStamp = Time.time;

            defaultColor.a = defaultColor.a == 0f ? 1f : 0f;

            Debug.Log(defaultColor.a);

            playerSprite.color = defaultColor;
        }
        
    }

    private void StartFlickering()
    {
        flickering = true;
    }

    private void StopFlickering()
    {
        flickering = false;
        defaultColor.a = 1f;
        playerSprite.color = defaultColor;
    }

    private void TakeDamage(int damage)
    {

        if (Time.time - invincibilityStartTime < invincibilityDuration)
            return;

        invincibilityStartTime = Time.time;

        if (FeedbackAnimationParameters.health - damage <= 0 )
        {
            FeedbackAnimationParameters.health = 0;

        }
        else
        {
            FeedbackAnimationParameters.health -= damage;
            StartFlickering();
            Invoke("StopFlickering",flickerOnDamageTime);
        }


    }


    /* -------------------------------------------------- END OF MISCELLEANOUS METHODS -------------------------------------------------- */





    /* -------------------------------------------------- BEGINNING OF UNITY METHODS -------------------------------------------------- */

    void OnBecameInvisible()
    {   
        ResetPlayerState();
        TakeDamage(1);
        Invoke("InitializePlayerAtCheckPointAfterDeath",0.05f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "CheckPoint")
        {
            respawnPoint = transform.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.SetParent(collision.transform, true);
            currentScale = new Vector3(initialScale.x / collision.transform.localScale.x, initialScale.y / collision.transform.localScale.y, initialScale.z / collision.transform.localScale.z);
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            this.transform.parent = null;
            currentScale = initialScale;
        }
    }



    /// <summary>
    /// Keeps track if a device (gamepad) was added or removed during runtime
    /// </summary>
    /// <param name="device"></param>
    /// <param name="change"></param>
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        // Check if the device is a gamepad
        if (device is Gamepad)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    // Gamepad connected
                    Debug.Log("Gamepad connected: " + device.name);
                    manette = Gamepad.current;
                    break;
                case InputDeviceChange.Removed:
                    // Gamepad disconnected
                    Debug.Log("Gamepad disconnected: " + device.name);
                    manette = null;
                    break;
                case InputDeviceChange.Reconnected:
                    // Gamepad reconnected
                    Debug.Log("Gamepad reconnected: " + device.name);
                    manette = Gamepad.current;
                    break;
                case InputDeviceChange.Disconnected:
                    // Gamepad temporarily disconnected
                    Debug.Log("Gamepad temporarily disconnected: " + device.name);
                    manette = null;
                    break;
            }
        }
    }


    /* -------------------------------------------------- END OF UNITY METHODS -------------------------------------------------- */

}
