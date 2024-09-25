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

    private Gamepad manette;

    private float tempStopGroundCheckTimer = 0.05f;
    private float jumpTimestamp = 0f;
    private bool previousGroundState;
    private bool isGrounded;
    private Rigidbody2D player;
    private Vector2 direction;
    private int jumpCounter;

    void Start()
    {
        // Subscribe to the device change event
        InputSystem.onDeviceChange += OnDeviceChange;


        player = GetComponent<Rigidbody2D>();
        manette = Gamepad.current;
        //jumpCounter = maxJumpAmount;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if ((isGrounded)&&(Time.time > jumpTimestamp + tempStopGroundCheckTimer))
        {
            jumpCounter = maxJumpAmount;
        }

        //previousGroundState = isGrounded;

        float move = Input.GetAxis("Horizontal") * moveSpeed;

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

        if (Input.GetButtonDown("Jump") && (jumpCounter>0))
        {
            player.velocity = new Vector2(player.velocity.x, jumpSpeed);
            jumpCounter--;
            jumpTimestamp = Time.time;
        }


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
