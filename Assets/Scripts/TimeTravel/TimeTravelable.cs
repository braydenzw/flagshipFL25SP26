using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravelable : MonoBehaviour
{
    
    List<Vector3> posRecord = new List<Vector3>();
    
    public void TimeTravel(int frame)
    {
        transform.position = posRecord[frame];
        posRecord.RemoveRange(frame + 1, posRecord.Count - frame - 1); // Removing all space afterwards. Not necessary, since traveling forward in time is banned
    }

    public void LogFrame()
    {
        posRecord.Add(transform.position);
    }
}
