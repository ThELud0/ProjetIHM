using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed;
    public float jumpSpeed;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius;
    public int maxJumpAmount;
    public float sprintSpeedCoef;
    public float doubleTapThreshold;

    private Gamepad manette;

    private float tempStopGroundCheckTimer = 0.05f;
    private float jumpTimestamp = 0f;
    private int jumpCounter;

    private float leftLastTapTime = 0f;
    private float rightLastTapTime = 0f;
    private bool sprinting;

    private bool previousGroundState;
    private bool isGrounded;

    private Rigidbody2D player;
    private Vector2 direction;


    void Start()
    {
        // Subscribe to the device change event
        InputSystem.onDeviceChange += OnDeviceChange;


        player = GetComponent<Rigidbody2D>();
        manette = Gamepad.current;

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if ((isGrounded)&&(Time.time > jumpTimestamp + tempStopGroundCheckTimer))
        {
            jumpCounter = maxJumpAmount;
        }


        float move = Input.GetAxis("Horizontal") * moveSpeed;

        move = CheckAndApplyPlayerHorizontalSprint(move);

        if ((move == 0)&&(manette != null))
        {
            direction = manette.dpad.ReadValue() * moveSpeed;
            if (!(direction == Vector2.zero))
                player.velocity = new Vector2(direction.x, player.velocity.y);
            else
                player.velocity = new Vector2(manette.leftStick.ReadValue().x * moveSpeed, player.velocity.y);
        }
        else
        {
            player.velocity = new Vector2(move, player.velocity.y);
        }

        if ((Input.GetButtonDown("Jump")) && (jumpCounter>0))
        {
            PlayerJumpUp();
        }
        else if (manette != null)
        {
            if ((manette.buttonSouth.wasPressedThisFrame) && (jumpCounter > 0))
            {
                PlayerJumpUp();
            }
        }

    }

    private void PlayerJumpUp()
    {
        player.velocity = new Vector2(player.velocity.x, jumpSpeed);
        jumpCounter--;
        jumpTimestamp = Time.time;
    }

    private float CheckAndApplyPlayerHorizontalSprint(float move)
    {
        // Check for a double tap on Q key
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) //Unity adapté à clavier QWERTY... donc Q = A :')
        {
            sprinting = false;
            if (Time.time - leftLastTapTime <= doubleTapThreshold)
            {
                sprinting = true;
            }
            leftLastTapTime = Time.time;
        }
        // Check for a double tap on D key
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            sprinting = false;
            if (Time.time - rightLastTapTime <= doubleTapThreshold)
            {
                sprinting = true;
            }
            rightLastTapTime = Time.time;
        }
        if (sprinting)
            move *= sprintSpeedCoef;

        return move;
    }

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
