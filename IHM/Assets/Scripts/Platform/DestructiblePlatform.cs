using UnityEngine;
using UnityEngine.InputSystem;

public class DestructiblePlatform : MonoBehaviour
{
    public float destructionTime = 1f;
    public float repairTime = 1.5f;
    private GameObject platformPrefab;

    public float vibrationDuration = 0.1f;   // Duration of vibration in seconds
    public float lowFrequency = 0.2f;        // Intensity for low-frequency motor (0 to 1)
    public float highFrequency = 0.2f;       // Intensity for high-frequency motor (0 to 1)

    private bool onPlatform = false;

    private Gamepad gamepad;

    private void Start()
    {
        platformPrefab = gameObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)//#TODO_N collision from under activates this too, check with the correct corner
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Invoke("DestroyPlatform", destructionTime);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        onPlatform = true;
        Vibrate();
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        StopVibration();
        onPlatform = false;
    }

    private void DestroyPlatform()
    {
        if (onPlatform)
        {
            StopVibration();
            onPlatform = false;
        }

        gameObject.SetActive(false);
        Invoke("RepairPlatform", repairTime);
    }

    private void RepairPlatform()
    {
        gameObject.SetActive(true);
    }

    // Call this function to give vibration feedback
    public void Vibrate()
    {
        gamepad = Gamepad.current;
        if (gamepad != null)
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
