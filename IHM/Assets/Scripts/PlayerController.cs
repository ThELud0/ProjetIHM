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

    private Gamepad manette;

    private bool isGrounded;
    private Rigidbody2D player;
    private Vector2 direction;


    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        manette = Gamepad.current;

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

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

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            player.velocity = new Vector2(player.velocity.x, jumpSpeed);
        }



    }
}
