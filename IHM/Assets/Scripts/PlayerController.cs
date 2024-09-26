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
    public float groundCheckRadius;
    public float climbCheckRadius;
    public int maxJumpAmount;
    public float sprintSpeedCoef;
    public float doubleTapTimeThreshold;
    public float normalGravityScale;   
    

    private Gamepad manette;

    private float tempStopGroundCheckTimer = 0.05f;
    private float tempStopClimbCheckTimer = 0.1f;
    private float jumpTimestamp = 0f;
    private int jumpCounter;
    private float noGravityScale = 0f;

    private float leftLastTapTime = 0f;
    private float rightLastTapTime = 0f;
    private bool sprinting;

    private bool previousGroundState;
    private bool isGrounded;

    private bool isClimbing;
    private bool jumpRefreshed;

    private Rigidbody2D player;
    private Vector2 direction;


    void Start()
    {
        // Subscribe to the device change event
        InputSystem.onDeviceChange += OnDeviceChange;


        player = GetComponent<Rigidbody2D>();
        manette = Gamepad.current;
        isClimbing = false;
        jumpRefreshed = false;
    }

    /* -------------------------------------------------- END OF START METHOD -------------------------------------------------- */

    // Update is called once per frame
    void Update()
    {
        // Check if there is ground under the player
        if ((Physics2D.OverlapCircle(playerLowerLeftCornerCheck.position, groundCheckRadius, groundLayer)) || 
            (Physics2D.OverlapCircle(playerLowerRightCornerCheck.position, groundCheckRadius, groundLayer)))
            isGrounded = true;
        else
            isGrounded = false;

        // Reset jump counter if player is on the ground
        if ((isGrounded) && (Time.time > jumpTimestamp + tempStopGroundCheckTimer)) //Stop checking for ground for a short time after initiating a jump to not reset the counter right away
            jumpCounter = maxJumpAmount;

        // Player is not submitted to gravity if climbing
        if (!isClimbing)
            player.gravityScale = normalGravityScale;
        else if (isClimbing)
            player.gravityScale = noGravityScale;
        

        // get player keyboard input for x-axis movement
        float move = Input.GetAxis("Horizontal") * moveSpeed;
        if ((move == 0)&&(manette != null)) //if no keyboard input check for gamepad input
        {
            direction = manette.dpad.ReadValue();
            if (direction == Vector2.zero)
                direction = manette.leftStick.ReadValue();
            move = direction.x * moveSpeed;

        }
        // check for sprinting input and modify move speed accordingly
        move = CheckAndApplyPlayerHorizontalSprint(move);
        // apply move speed to player velocity
        player.velocity = new Vector2(move, player.velocity.y);

        // check if player is near climbable surface
        if (CheckAndReturnIfPlayerCanClimb())
        {
            //check if player gives climbing input
            CheckClimbInputAndSet();
            if (isClimbing  && (Time.time > jumpTimestamp + tempStopClimbCheckTimer))
            {
                // get player keyboard input for y-axis movement
                float verticalMove = Input.GetAxis("Vertical") * climbSpeed;
                if ((verticalMove == 0) && (manette != null)) //if no keyboard input check for gamepad input
                {
                    direction = manette.dpad.ReadValue();
                    if (direction == Vector2.zero)
                        direction = manette.leftStick.ReadValue();
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

        //check for jump input and make player jump if there are jumps remaining in jump counter
        if ((Input.GetButtonDown("Jump")) && (jumpCounter>0))
        {
            PlayerJumpUp();
        }
        else if (manette != null) 
        {
            // check gamepad jump input
            if ((manette.buttonSouth.wasPressedThisFrame) && (jumpCounter > 0))
            {
                PlayerJumpUp();
            }
        }

    }

    /* -------------------------------------------------- END OF UPDATE METHOD -------------------------------------------------- */


    /// <summary>
    /// Make player jump up
    /// </summary>
    private void PlayerJumpUp()
    {
        player.velocity = new Vector2(player.velocity.x, jumpSpeed);
        jumpCounter--;
        jumpTimestamp = Time.time;
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
            if (manette.leftShoulder.isPressed)
                sprinting = true;
            else if (manette.leftShoulder.wasReleasedThisFrame)
                sprinting = false;
        }

        // Check for a double tap on Q key
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) //Unity adapté à clavier QWERTY... donc Q = A :')
        {
            sprinting = false;
            if (Time.time - leftLastTapTime <= doubleTapTimeThreshold)
            {
                sprinting = true;
            }
            leftLastTapTime = Time.time;
        }
        // Check for a double tap on D key
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            sprinting = false;
            if (Time.time - rightLastTapTime <= doubleTapTimeThreshold)
            {
                sprinting = true;
            }
            rightLastTapTime = Time.time;
        }


        if (sprinting)
            move *= sprintSpeedCoef;

        return move;
    }


    /// <summary>
    /// Checks if a climbing input was given by the player and calls climbing methods if so
    /// </summary>
    private void CheckClimbInputAndSet()
    {
        if ((Input.GetKeyUp(KeyCode.LeftShift)) || (Input.GetKeyUp(KeyCode.RightShift)) )
        {
            NotClimbingOrStopped();
        }
        if ((Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.RightShift)))
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

}
