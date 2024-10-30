using System.Diagnostics;
using UnityEngine;

public class MoveCameraSideB : MonoBehaviour
{
    public Transform targetCameraPos;
    public Transform previousCameraPos;
    public float cameraMoveSpeed = 2f;

    private Transform currentTarget;
    private bool isMoving = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float distanceToPrevious = Vector3.Distance(Camera.main.transform.position, previousCameraPos.position);
            float distanceToTarget = Vector3.Distance(Camera.main.transform.position, targetCameraPos.position);

            currentTarget = distanceToPrevious < distanceToTarget ? targetCameraPos : previousCameraPos;
            isMoving = true;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, currentTarget.position, cameraMoveSpeed * Time.deltaTime);

            if (Vector3.Distance(Camera.main.transform.position, currentTarget.position) < 0.05f)
            {
                Camera.main.transform.position = currentTarget.position;
                isMoving = false;
            }

        }
    }
}
