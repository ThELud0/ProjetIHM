using UnityEngine;

public class DestructiblePlatform : MonoBehaviour
{
    public float destructionTime = 1f;
    public float repairTime = 1.5f;
    private GameObject platformPrefab;

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

    private void DestroyPlatform()
    {
        gameObject.SetActive(false);
        Invoke("RepairPlatform", repairTime);
    }

    private void RepairPlatform()
    {
        gameObject.SetActive(true);
    }
}
