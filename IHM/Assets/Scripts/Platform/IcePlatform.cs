using UnityEngine;

public class IcePlatform : MonoBehaviour
{
    public float iceSpeedMultiplier = 1.5f;
    public float iceControlReduction = 0.2f;
    public float normalSpeed = 10f; //#TODO_N just get actual current player speed ?

    private Rigidbody2D playerRigidbody;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRigidbody != null)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                player.moveSpeed *= iceSpeedMultiplier;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerRigidbody != null)
            {
                float moveInput = Input.GetAxis("Horizontal");

                playerRigidbody.linearVelocity = new Vector2(
                    Mathf.Lerp(playerRigidbody.linearVelocity.x, moveInput * playerRigidbody.linearVelocity.magnitude, iceControlReduction),
                    playerRigidbody.linearVelocity.y
                );
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
                player.moveSpeed = normalSpeed;
        }
    }
}
