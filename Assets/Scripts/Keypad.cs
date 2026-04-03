using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour
{
    // Start is called before the first frame update
    
    String inputs;
    public String key = "0000";


    public 
    void Start()
    {
        inputs = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonInput(int buttonNumber)
    {
        if(buttonNumber == ButtonConsts.RESET)
        {
            inputs = "";
        }
        else if (buttonNumber == ButtonConsts.ENTER)
        {
            if(inputs==key)
            {
                Output();
            }
        }
        else
        {
            handleInput(buttonNumber);
        }
    }

    public void handleInput(int buttonNumber)
    {
        if(inputs.Length < key.Length)
        {
            inputs = inputs + buttonNumber;
        }
        Debug.Log(inputs);
    }    

    public void Output()
    {
        Debug.Log("Keypad Output");
    }

}
