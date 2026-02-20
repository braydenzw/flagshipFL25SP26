using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Transform playerCamera;
    public float pickupRange = 3f;
    public float throwPower = 10f;
    public Transform holdPoint;
    public float maxDoorHoldDistance = 4f;

    private GameObject heldItem;
    private Rigidbody heldItemRb;
    private Door currentlyHeldDoor;

    [Header("UI Settings")]
    public RawImage interactionStateImage;
    public Texture emptyHandTexture;
    public Texture holdingItemTexture;
    public Texture throwingItemTexture;

    void Start()
    {
        UpdateInteractionImage(1);
    }

    void Update()
    {
        if (currentlyHeldDoor != null)
        {
            float distanceToDoor = Vector3.Distance(playerCamera.position, currentlyHeldDoor.transform.position);

            if (Input.GetMouseButtonUp(0) || distanceToDoor > maxDoorHoldDistance)
            {
                currentlyHeldDoor = null;
                return;
            }

            float mouseX = -Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");
            float mouseMovement = mouseX + mouseY;

            currentlyHeldDoor.ManipulateDoor(mouseMovement);

            return;
        }

        if (heldItem == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, pickupRange))
            {
                Item itemScript = hit.transform.GetComponent<Item>();
                Door doorScript = hit.transform.GetComponentInParent<Door>();

                if (itemScript != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        PickupItem(hit.transform.gameObject);
                    }
                }
                else if (doorScript != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        currentlyHeldDoor = doorScript;
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(1))
            {
                ThrowItem();
            }

            if (Input.GetMouseButtonUp(0))
            {
                DropItem();
            }
        }
    }

    void PickupItem(GameObject item)
    {
        heldItem = item;
        heldItemRb = item.GetComponent<Rigidbody>();

        if (heldItemRb != null)
        {
            heldItemRb.isKinematic = true;
        }

        heldItem.transform.SetParent(holdPoint);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        if (item.GetComponent<Collider>() != null)
        {
            item.GetComponent<Collider>().enabled = false;
        }

        UpdateInteractionImage(2);
    }

    void ThrowItem()
    {
        if (heldItemRb != null)
        {
            heldItemRb.isKinematic = false;
            heldItemRb.AddForce(playerCamera.forward * throwPower, ForceMode.Impulse);
        }

        heldItem.transform.SetParent(null);

        if (heldItem.GetComponent<Collider>() != null)
        {
            heldItem.GetComponent<Collider>().enabled = true;
        }

        heldItem = null;
        heldItemRb = null;

        UpdateInteractionImage(3);
    }

    void DropItem()
    {
        if (heldItemRb != null)
        {
            heldItemRb.isKinematic = false;
            heldItemRb.AddForce(playerCamera.forward * 2f, ForceMode.Impulse);
        }

        heldItem.transform.SetParent(null);

        if (heldItem.GetComponent<Collider>() != null)
        {
            heldItem.GetComponent<Collider>().enabled = true;
        }

        heldItem = null;
        heldItemRb = null;

        UpdateInteractionImage(1);
    }

    void UpdateInteractionImage(int isHolding)
    {
        if (interactionStateImage == null) return;

        if (isHolding == 1)
        {
            interactionStateImage.texture = emptyHandTexture;
        }
        else if (isHolding == 2)
        {
            interactionStateImage.texture = holdingItemTexture;
        }
        else if (isHolding == 3)
        {
            interactionStateImage.texture = throwingItemTexture;
        }
    }
}