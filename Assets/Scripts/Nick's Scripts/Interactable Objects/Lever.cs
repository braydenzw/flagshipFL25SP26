using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable
{
    [SerializeField] private GameObject lever;
    private bool isLeverPulled;

    //This funciton is where we will design our interaction using code
    protected override void Interact()
    {
        isLeverPulled = !isLeverPulled;
        lever.GetComponent<Animator>().SetBool("IsPulled", isLeverPulled);
        //Debug.Log("Interacted with " + gameObject.name);
    }
}
