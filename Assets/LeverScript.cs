using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Author: Peter Jacobsen | Github: peterjws3
// This script handles the lever motion when clicked and stuff
public class LeverScript : MonoBehaviour
{
    float cur_time;
    bool flipped;
    bool rotating;
    Quaternion next_angle_transform;

    float next_angle;
    float rotation_speed = 6;

    // Start is called before the first frame update
    void Start()
    {
        cur_time = 0;
        flipped = false;
        rotating = false;
        next_angle = transform.eulerAngles.z;
        next_angle_transform = Quaternion.Euler(0, 0, next_angle);
    }

    // Update is called once per frame
    void Update()
    {
        cur_time += Time.deltaTime;
        
        // Test code here
        if (cur_time >= 2)
        {
            Clicked();
            cur_time -= 2;
        }
        Rotate();
        // Basic state management pretty much
        if (rotating)
        {
            Debug.Log(Mathf.DeltaAngle(transform.eulerAngles.z, next_angle));
            if (Math.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, next_angle))<2)
            {
                rotating = false;
                print("I am done rotating now :)");
            }
        }

    }

    void Rotate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, next_angle_transform, rotation_speed * Time.deltaTime);
    }

    public void Clicked()
    {
        print("clciked");
        if (!rotating)
        {
            rotating = true;
            next_angle = -next_angle;
            next_angle_transform = Quaternion.Euler(0, 0, next_angle);
            Debug.Log(next_angle_transform.eulerAngles.z);
        }
    }


}
