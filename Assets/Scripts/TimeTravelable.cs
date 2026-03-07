using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TimeTravelable : MonoBehaviour
{
    [SerializeField] private bool positionSetOnTravel;
    [SerializeField] private bool velocitySetOnTravel;

    List<Vector3> posRecord = new List<Vector3>();
    List<Vector3> velRecord = new List<Vector3>();
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) velocitySetOnTravel = false;
    }

    public void TimeTravel(int frame)
    {
        if (positionSetOnTravel)
        {
            transform.position = posRecord[frame];
            posRecord.RemoveRange(frame + 1, posRecord.Count - frame - 1); // Removing all space afterwards. Not necessary, since traveling forward in time is banned
        }
        if (velocitySetOnTravel)
        {
            rb.velocity = velRecord[frame];
            velRecord.RemoveRange(frame + 1, velRecord.Count - frame - 1);

        }
    }

    public void LogFrame()
    {
        posRecord.Add(transform.position);
        if (rb != null)
        {
            velRecord.Add(rb.velocity);
        }
        else
        {
            velRecord.Add(Vector3.zero);
        }
    }
}
