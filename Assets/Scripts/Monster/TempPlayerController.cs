using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// attached to player, will rotate transform
// since camera is a child of player, will rotate that too
public class TempPlayerController : MonoBehaviour
{
    private float rotationX = 0f;
    private float rotationY = 0f;
    public float sensitivity = 10f;
    public float speed = 5f;

    private Rigidbody rb;
    private float moveX;
    private float moveZ;

    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        // Lock and hide the cursor for mouse look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        rotationX += Input.GetAxis("Mouse Y") * -sensitivity;
        rotationY += Input.GetAxis("Mouse X") * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
    }

    void FixedUpdate() {
        // Calculate movement direction relative to world space
        Vector3 move = (transform.forward * moveZ + transform.right * moveX).normalized;
        rb.velocity = move * speed;
    }
}
