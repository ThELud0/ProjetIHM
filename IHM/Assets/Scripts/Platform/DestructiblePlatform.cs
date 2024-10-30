using UnityEngine;
using UnityEngine.InputSystem;

public class DestructiblePlatform : MonoBehaviour
{
    public float destructionTime = 1f;
    public float repairTime = 1.5f;
    public AudioClip breakingSoundClip;
    public AudioClip crumblingSoundClip;
    private GameObject platformPrefab;

    public float lowFrequency ;        // Intensity for low-frequency motor (0 to 1)
    public float highFrequency;       // Intensity for high-frequency motor (0 to 1)

    //public float yMinShakingOffset;
    //public float xMinShakingOffset;
    public float yMaxShakingOffset;
    public float xMaxShakingOffset;

    private bool onPlatform = false;

    private Gamepad gamepad;
    private Vector2 initialPosition;
    private float xShakingOffset;
    private float yShakingOffset;
    private bool crumbling = false;

    private void Start()
    {
        platformPrefab = gameObject;
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (crumbling && FeedbackAnimationParameters.crumblingPlatformShakeAnimation)
        {
            xShakingOffset = Random.Range(-xMaxShakingOffset, xMaxShakingOffset);
            yShakingOffset = Random.Range(-yMaxShakingOffset, yMaxShakingOffset);
            transform.position = new Vector2(initialPosition.x + xShakingOffset, initialPosition.y + yShakingOffset);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)//#TODO_N collision from under activates this too, check with the correct corner
    {
        if ((collision.gameObject.CompareTag("Player"))&&!crumbling)
        {
            SoundFXManager.instance.PlaySoundFXClipSpecificLength(crumblingSoundClip, transform, 1f, destructionTime);
            Invoke("DestroyPlatform", destructionTime);
        }
        crumbling = true;
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        onPlatform = true;
        Vibrate();
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Invoke("StopVibration", 0.2f);
        onPlatform = false;
    }

    private void DestroyPlatform()
    {
        if (onPlatform)
        {
            Invoke("StopVibration",0.3f);
            onPlatform = false;
        }
        SoundFXManager.instance.PlaySoundFXClip(breakingSoundClip, transform, 1f);

        gameObject.SetActive(false);
        crumbling = false;
        Invoke("RepairPlatform", repairTime);
    }

    private void RepairPlatform()
    {
        gameObject.SetActive(true);
        transform.position = initialPosition;
    }

    // Call this function to give vibration feedback
    public void Vibrate()
    {
        gamepad = Gamepad.current;
        if ((gamepad != null)&&FeedbackAnimationParameters.crumblingPlatformControllerShakeAnimation)
        {
            // Set gamepad vibration for both low and high-frequency motors
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);

        }
        
    }

    // Stop the vibration
    private void StopVibration()
    {
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0); // Stop vibration
        }
        
    }


}
