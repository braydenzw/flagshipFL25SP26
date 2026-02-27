using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Transform playerCamera;
    public float pickupRange = 3f;
    public Transform holdPoint;
    public float maxDoorHoldDistance = 4f;

    [Header("Throw Settings")]
    public float minThrowPower = 1f;
    public float maxThrowPower = 10f;
    public float maxChargeTime = 1.5f;
    private float currentChargeTime;
    private bool isCharging = false;

    private GameObject heldItem;
    private Rigidbody heldItemRb;
    private Door currentlyHeldDoor;

    [Header("UI Settings")]
    public RawImage interactionStateImage;
    public Texture emptyHandTexture;
    public Texture holdingItemTexture;
    public Texture throwingItemTexture;
    public Texture doorGrabTexture;
    public Slider chargeBar;

    void Start()
    {
        UpdateInteractionImage(1);

        if (chargeBar != null)
        {
            chargeBar.gameObject.SetActive(false);
            chargeBar.minValue = 0;
            chargeBar.maxValue = 1;
        }
    }

    void Update()
    {
        // Door controls
        if (currentlyHeldDoor != null)
        {
            float distanceToDoor = Vector3.Distance(playerCamera.position, currentlyHeldDoor.transform.position);

            if (Input.GetMouseButtonUp(0) || distanceToDoor > maxDoorHoldDistance)
            {
                currentlyHeldDoor = null;
                UpdateInteractionImage(1);
                return;
            }

            float mouseX = -Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");
            float mouseMovement = mouseX + mouseY;

            currentlyHeldDoor.ManipulateDoor(mouseMovement);
            return;
        }

        // Pickup controls
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
                        UpdateInteractionImage(4);
                    }
                }
            }
        }

        else
        {
            HandleThrowingLogic();

            if (Input.GetMouseButtonUp(0))
            {
                CancelCharge();
                DropItem();
            }
        }
    }

    private void HandleThrowingLogic()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isCharging = true;
            currentChargeTime = 0f;
            if (chargeBar != null) chargeBar.gameObject.SetActive(true);
        }

        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
            float chargePercent = Mathf.Clamp01(currentChargeTime / maxChargeTime);

            if (chargeBar != null)
            {
                chargeBar.value = chargePercent;
            }
        }

        if (Input.GetMouseButtonUp(1) && isCharging)
        {
            ThrowItem();
        }
    }

    private void CancelCharge()
    {
        isCharging = false;
        if (chargeBar != null) chargeBar.gameObject.SetActive(false);
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
        float chargePercent = Mathf.Clamp01(currentChargeTime / maxChargeTime);
        float finalPower = Mathf.Lerp(minThrowPower, maxThrowPower, chargePercent);

        if (heldItemRb != null)
        {
            heldItemRb.isKinematic = false;
            heldItemRb.AddForce(playerCamera.forward * finalPower, ForceMode.Impulse);
        }

        ResetItemReferences();
        CancelCharge();
        UpdateInteractionImage(3);
    }

    void DropItem()
    {
        if (heldItemRb != null)
        {
            heldItemRb.isKinematic = false;
            heldItemRb.AddForce(playerCamera.forward * 2f, ForceMode.Impulse);
        }

        ResetItemReferences();
        UpdateInteractionImage(1);
    }

    private void ResetItemReferences()
    {
        if (heldItem == null) return;

        heldItem.transform.SetParent(null);

        if (heldItem.GetComponent<Collider>() != null)
        {
            heldItem.GetComponent<Collider>().enabled = true;
        }

        heldItem = null;
        heldItemRb = null;
    }

    void UpdateInteractionImage(int isHolding)
    {
        if (interactionStateImage == null)
        {
            return;
            }

        if (isHolding == 1) { 
            interactionStateImage.texture = emptyHandTexture; // Idle hands
            }
        else if (isHolding == 2) { 
            interactionStateImage.texture = holdingItemTexture; // Grabbing hands
            }
        else if (isHolding == 3) { 
            interactionStateImage.texture = throwingItemTexture; // Throwing hand
            }
        else if (isHolding == 4) { 
            interactionStateImage.texture = doorGrabTexture; // Door-grabbing hand
            }
        }
}