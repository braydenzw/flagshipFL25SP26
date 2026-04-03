using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPointRotation : MonoBehaviour
{
    public Transform playerCamera;

    [Header("Hold Settings")]
    public float holdDistance = 1.5f;

    void Update()
    {
        if (playerCamera != null)
        {
            // This is the camera's position plus a vector pointing forward from the camera.
            Vector3 targetPosition = playerCamera.position + playerCamera.forward * holdDistance;

            // Set the position of this object (the hold point).
            transform.position = targetPosition;

            // Set the rotation of this object to match the camera's rotation.
            transform.rotation = playerCamera.rotation;
        }
    }
}
