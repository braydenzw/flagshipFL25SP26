using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public Vector3 Pos { get; private set; }
    public Quaternion Rot {get; private set;}

    public void Inst(Transform transform)
    {
        Pos = transform.position;
        Rot = transform.rotation;
    }
}
