using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wardrobe : Interactable
{
    [SerializeField] private string customCloseMessage;
    [SerializeField] private Animator animator;

    private bool isOpen = false;
    private string originalPrompt;

    void Start()
    {
        originalPrompt = promptMessage;
    }

    protected override void Interact()
    {
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);

        if (isOpen)
        {
            promptMessage = customCloseMessage;
        }
        else
        {
            promptMessage = originalPrompt;
        }
    }

}
