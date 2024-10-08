using UnityEngine;

public class WindZone : MonoBehaviour
{
    public float floatingGravityScale = -1.5f;
    public float originalGravityScale;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            originalGravityScale = player.normalGravityScale;
            player.normalGravityScale = floatingGravityScale;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
            player.normalGravityScale = originalGravityScale;
    }
}