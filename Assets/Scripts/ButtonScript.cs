using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    // while Interact, clicked will not update and will stay pressed, 
    // if not then clicked will update after animation done
    float outY;
    float speed = 15;
    bool interacting;
    
    void Start()
    {
        outY = transform.localPosition.y;
        interacting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("down"))
        {
            Interact();
        }
        if (interacting)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, 0), speed * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, outY, 0), speed * Time.deltaTime);

        }

        interacting = false;
    }

    public void Interact() 
    {
        interacting = true;
    }
    public void Output()
    {
        
    }
}
