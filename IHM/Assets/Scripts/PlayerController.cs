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

    private float initJumpTimer = 0.05f;
    private bool previousGroundState;
    private bool isGrounded;
    private Rigidbody2D player;
    private Vector2 direction;
    private int jumpCounter;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        manette = Gamepad.current;
        //jumpCounter = maxJumpAmount;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            jumpCounter = maxJumpAmount;
        }

        //previousGroundState = isGrounded;

        float move = Input.GetAxis("Horizontal") * moveSpeed;

        if (move == 0)
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

        }

        


    }
}
