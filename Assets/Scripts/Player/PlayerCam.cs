using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public Transform orientation;

    [Header("Sensitivity")]
    public float sensX = 100f;
    public float sensY = 100f;

    private float xRotation;
    private float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;

        
        yRotation += mouseX;
        // Subtracting because positive Y mouse movement should mean looking up.
        xRotation -= mouseY;
                
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
