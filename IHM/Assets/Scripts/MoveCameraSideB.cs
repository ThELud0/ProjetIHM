using System.Diagnostics;
using UnityEngine;

public class MoveCameraSideB : MonoBehaviour
{
    public Transform targetCameraPos;
    public Transform previousCameraPos;
    public float cameraMoveSpeed = 4f;
    private Vector2 triggerPos;

    private Transform currentTarget;
    private bool isMoving = false;

    private Vector2 enterPosition;
    private Vector2 exitPosition;

    private void Start()
    {
        triggerPos = GetComponent<Transform>().position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float distanceToPrevious = Vector3.Distance(Camera.main.transform.position, previousCameraPos.position);
            float distanceToTarget = Vector3.Distance(Camera.main.transform.position, targetCameraPos.position);

            enterPosition = collision.transform.position;

            currentTarget = distanceToPrevious < distanceToTarget ? targetCameraPos : previousCameraPos;
            isMoving = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            exitPosition = collision.transform.position;


            bool samesign = (triggerPos.x - enterPosition.x < 0) == (triggerPos.x - exitPosition.x < 0);
            if (samesign)
                currentTarget = currentTarget == targetCameraPos? previousCameraPos : targetCameraPos;

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
