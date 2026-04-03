using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    public GameObject puzzleBoard;
    public Camera mainCamera;
    public Transform zoomTarget;
    public float zoomSpeed = 5f;

    private bool isZoomed = false;
    private Vector3 originalPos;
    private Quaternion originalRot;

    void Update()
    {
        // Press E to enter the puzzle
        if (Input.GetKeyDown(KeyCode.E) && !isZoomed)
        {
            EnterPuzzle();
        }
    }

    void EnterPuzzle()
    {
        originalPos = mainCamera.transform.position;
        originalRot = mainCamera.transform.rotation;
        isZoomed = true;
        puzzleBoard.SetActive(true);
    }

    void LateUpdate()
    {
        if (isZoomed)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, zoomTarget.position, Time.deltaTime * zoomSpeed);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, zoomTarget.rotation, Time.deltaTime * zoomSpeed);
        }
    }

    public void ExitPuzzle()
    {
        isZoomed = false;
        mainCamera.transform.position = originalPos;
        mainCamera.transform.rotation = originalRot;
        puzzleBoard.SetActive(false);
    }
}