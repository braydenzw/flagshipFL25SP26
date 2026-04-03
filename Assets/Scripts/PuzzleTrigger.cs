using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera mainCamera;
    public Transform zoomTarget;
    public float zoomSpeed = 5f;

    [Tooltip("Interaction Distance")]
    public float interactionRange = 3f;

    [Header("Toggled Scripts")]
    public PlayerMovement playerMovement;
    public PlayerInteraction playerInteraction;
    public FlowManager flowManager;

    [Tooltip("Puzzle Disabled Scripts")]
    public MonoBehaviour[] cameraScriptsToDisable;

    [Header("Physics Settings")]
    [Tooltip("Flow Game Rigidbody.")]
    public Rigidbody puzzleRigidbody;

    private bool isZoomingIn = false;
    private bool isPlaying = false;
    private bool isSolved = false;

    private Vector3 originalPos;
    private Quaternion originalRot;

    void Start()
    {
        if (flowManager != null)
        {
            flowManager.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isPlaying && !isSolved)
        {
            TryInteract();
        }
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)) && isPlaying)
        {
            ExitPuzzle();
        }
    }

    void TryInteract()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.transform == this.transform)
            {
                EnterPuzzle();
            }
        }
    }

    void EnterPuzzle()
    {
        isPlaying = true;
        isZoomingIn = true;

        originalPos = mainCamera.transform.position;
        originalRot = mainCamera.transform.rotation;

        if (playerMovement != null)
        {
            playerMovement.SetMovementState(false);
        }

        if (playerInteraction != null)
        {
            playerInteraction.SetInteractionState(false);
        }

        foreach (MonoBehaviour script in cameraScriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        if (flowManager != null)
        {
            flowManager.enabled = true;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (isZoomingIn)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, zoomTarget.position, Time.deltaTime * zoomSpeed);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, zoomTarget.rotation, Time.deltaTime * zoomSpeed);
        }
    }

    public void ExitPuzzle()
    {
        isPlaying = false;
        isZoomingIn = false;

        if (flowManager != null)
        {
            flowManager.CancelActiveDrawing();
            flowManager.enabled = false;
        }

        mainCamera.transform.position = originalPos;
        mainCamera.transform.rotation = originalRot;

        if (playerMovement != null)
        {
            playerMovement.SetMovementState(true);
        }

        if (playerInteraction != null)
        {
            playerInteraction.SetInteractionState(true);
        }

        foreach (MonoBehaviour script in cameraScriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MarkAsSolved()
    {
        isSolved = true;
        ExitPuzzle();
        Debug.Log("Puzzle locked: cannot be accessed again.");

        // Make the puzzle fall
        if (puzzleRigidbody != null)
        {
            puzzleRigidbody.isKinematic = false;
            puzzleRigidbody.useGravity = true;

        }
    }
}