using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float dragSensitivity = 2f;

    [Header("Physics Settings")]
    public float friction = 4f;
    public float doorWeight = 2f;

    private float currentAngle = 0f;
    private float angularVelocity = 0f;
    private Quaternion closedRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
    }

    void Update()
    {
        currentAngle += angularVelocity * Time.deltaTime;

        float minAngle = Mathf.Min(0f, openAngle);
        float maxAngle = Mathf.Max(0f, openAngle);

        if (currentAngle <= minAngle)
        {
            currentAngle = minAngle;
            angularVelocity = 0f;
        }
        else if (currentAngle >= maxAngle)
        {
            currentAngle = maxAngle;
            angularVelocity = 0f;
        }

        angularVelocity = Mathf.Lerp(angularVelocity, 0f, Time.deltaTime * friction);

        transform.localRotation = closedRotation * Quaternion.Euler(0, currentAngle, 0);
    }

    public void ManipulateDoor(float mouseMovement)
    {
        angularVelocity += (mouseMovement * dragSensitivity * 100f) / doorWeight;
    }
}