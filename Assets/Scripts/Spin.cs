using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public Vector3 rotation_speed = new Vector3(0, 100, 0);

    void Update()
    {
        // Rotate the object every frame based on real-time
        transform.Rotate(rotation_speed * Time.deltaTime);
    }
}
