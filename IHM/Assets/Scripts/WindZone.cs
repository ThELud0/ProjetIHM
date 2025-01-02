using UnityEngine;

public class WindZone : MonoBehaviour
{
    public float floatingGravityScale = -1.5f;
    public float originalGravityScale;
    public AudioClip windSoundClip;

    [SerializeField] private SerialHandler serialHandler;

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
            //#TODO_N fonctionne très bien mais créé quand même une erreur, voir pk
            serialHandler.SendMessage('E', null);
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
            serialHandler.SendMessage('X', null);
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