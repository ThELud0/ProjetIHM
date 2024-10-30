using UnityEngine;

public class WindZone : MonoBehaviour
{
    public float floatingGravityScale = -1.5f;
    public float originalGravityScale;
    public AudioClip windSoundClip;

    public float clipLoopTimer;

    private float clipStartTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            originalGravityScale = player.normalGravityScale;
            player.normalGravityScale = floatingGravityScale;
            clipStartTime = Time.time;
            SoundFXManager.instance.PlaySoundFXClipSpecificLength(windSoundClip, transform, 1f, clipLoopTimer);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {

            player.normalGravityScale = originalGravityScale;
            clipStartTime = Time.time;
            SoundFXManager.instance.PlaySoundFXClipSpecificLength(windSoundClip, transform, 1f, clipLoopTimer);
        }
            
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if ((player != null) && (Time.time - clipStartTime >= clipLoopTimer))
        {
            clipStartTime = Time.time;
            SoundFXManager.instance.PlaySoundFXClipSpecificLength(windSoundClip, transform, 1f, clipLoopTimer);

        }
    }


}