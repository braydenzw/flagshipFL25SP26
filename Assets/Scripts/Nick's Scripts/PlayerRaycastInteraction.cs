using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class PlayerRaycastInteraction : MonoBehaviour
{
    public Camera playerCamera;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask mask;
    private PlayerPromptMessageUI playerUI;
    //private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        playerUI = GetComponent<PlayerPromptMessageUI>();
        //inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        playerUI.UpdateText(string.Empty);

        //Create a ray at the center of the camera, shooting outwards
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactionRange);
        RaycastHit hitInfo; //Variable to store out collision information
        if (Physics.Raycast(ray, out hitInfo, interactionRange, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.BaseInteract();
                }
                //Debug.Log(hitInfo.collider.GetComponent<Interactable>().promptMessage);
            }
        }
    }
}
