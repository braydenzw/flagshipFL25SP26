//// PlayerInteraction.cs
//using UnityEngine;
//using static UnityEditor.Progress;

//public class PlayerInteraction : MonoBehaviour
//{
//    [Header("Interaction Settings")]
//    public Transform playerCamera;
//    public float pickupRange = 3f;
//    public KeyCode pickupKey = KeyCode.E;
//    public KeyCode dropKey = KeyCode.Q;
//    // I just set the left mouse button as how to throw items, but there's this if you want a keystroke: public KeyCode throwKey = KeyCode.R;
//    public float throwPower = 10f;
//    public Transform holdPoint;

//    private GameObject heldItem;
//    private Rigidbody heldItemRb;

//    void Update()
//    {
//        if (heldItem == null)
//        {
//            RaycastHit hit;
//            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, pickupRange))
//            {
//                if (hit.transform.GetComponent<Item>() != null)
//                {
//                    if (Input.GetKeyDown(pickupKey))
//                    {
//                        PickupItem(hit.transform.gameObject);
//                    }
//                }
//            }
//        }
//        else
//        {
//            if (Input.GetKeyDown(dropKey))
//            {
//                DropItem();
//            }
//            if (Input.GetMouseButton(0))
//            {
//                ThrowItem();
//            }
//        }
//    }

//    void PickupItem(GameObject item)
//    {
//        heldItem = item;
//        heldItemRb = item.GetComponent<Rigidbody>();

//        heldItemRb.isKinematic = true;
//        heldItem.transform.SetParent(holdPoint);
//        heldItem.transform.localPosition = Vector3.zero;
//        heldItem.transform.localRotation = Quaternion.identity;

//        if (item.GetComponent<Collider>() != null)
//        {
//            item.GetComponent<Collider>().enabled = false;
//        }
//    }

//    void ThrowItem()
//    {
//        heldItemRb.isKinematic = false;
//        heldItem.transform.SetParent(null);

//        if (heldItem.GetComponent<Collider>() != null)
//        {
//            heldItem.GetComponent<Collider>().enabled = true;
//        }

//        heldItemRb.AddForce(playerCamera.forward * throwPower, ForceMode.Impulse);

//        heldItem = null;
//        heldItemRb = null;
//    }
//    void DropItem()
//    {
//        heldItemRb.isKinematic = false;
//        heldItem.transform.SetParent(null);

//        if (heldItem.GetComponent<Collider>() != null)
//        {
//            heldItem.GetComponent<Collider>().enabled = true;
//        }

//        heldItemRb.AddForce(playerCamera.forward, ForceMode.Impulse);

//        heldItem = null;
//        heldItemRb = null;
//    }
//}

// PlayerInteraction.cs
using UnityEngine;
using UnityEngine.UI;


public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Transform playerCamera;
    public float pickupRange = 3f;

    // public KeyCode throwKey = KeyCode.Q;
    public float throwPower = 10f;
    public Transform holdPoint;

    private GameObject heldItem;
    private Rigidbody heldItemRb;

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
        if (heldItem == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, pickupRange))
            {
                if (hit.transform.GetComponent<Item>() != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        PickupItem(hit.transform.gameObject);
                    }
                }
            }
        }
        else
        {
            // I set it to right click, but it could also be a keypress for something like 'q'.
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

        heldItemRb.isKinematic = true;

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
        heldItemRb.isKinematic = false;
        heldItem.transform.SetParent(null);

        if (heldItem.GetComponent<Collider>() != null)
        {
            heldItem.GetComponent<Collider>().enabled = true;
        }

        heldItemRb.AddForce(playerCamera.forward * throwPower, ForceMode.Impulse);

        heldItem = null;
        heldItemRb = null;

        UpdateInteractionImage(3);
    }

    void DropItem()
    {
        heldItemRb.isKinematic = false;
        heldItem.transform.SetParent(null);

        // Re-enable the collider
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