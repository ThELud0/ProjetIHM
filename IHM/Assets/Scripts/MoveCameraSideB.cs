using UnityEngine;
using System.Collections;

public class MoveCameraSideB : MonoBehaviour
{
    public Transform targetPosition;
    public float cameraMoveSpeed = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Vector3.Distance(Camera.main.transform.position, targetPosition.position) != 0)
        {
            StartCoroutine(MoveCamera());
        }
    }

    private IEnumerator MoveCamera()
    {
        while (Vector3.Distance(Camera.main.transform.position, targetPosition.position) > 0.05f)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition.position, cameraMoveSpeed * Time.deltaTime);
            yield return null;
        }
        Camera.main.transform.position = targetPosition.position;
    }
}
