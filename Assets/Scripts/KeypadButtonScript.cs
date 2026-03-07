using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

public class KeypadButtonScript : ButtonScript
{
    Keypad keypad;
    int buttonNumber;

    protected override void Start()
    {
        base.Start();
        keypad = GetComponentInParent<Keypad>();
    }
    protected override void Output()
    {
        keypad.buttonInput(buttonNumber);
        
    }
    void Awake()
    {
        buttonNumber = transform.parent.parent.GetSiblingIndex()-2;
        Debug.Log(buttonNumber);
        if(buttonNumber==ButtonConsts.RESET) {
            Renderer r = GetComponent<Renderer>();
            r.material.color = Color.red;
        }
        else if(buttonNumber==ButtonConsts.ENTER) {
            Renderer r = GetComponent<Renderer>();
            r.material.color = Color.green;
        }
    }   

}
