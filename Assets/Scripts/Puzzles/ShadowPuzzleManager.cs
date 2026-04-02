using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

// inherits MatchTwoObjectRotations for shadow puzzles (please refer to that script)
// IMPORTANT: attach to the GameObject that will be rotated for the puzzle

// IMPORTANT: this object (the one you rotate) and o2 (the object to compare to) MUST BE IDENTICAL (except for rotation)

// IMPORTANT: for o2, if you want it to be INSVISIBLE but STILL CAST SHADOWS:
// go to: o2 -> Mesh Renderer -> Cast Shadows -> Shadows Only

// ALSO IMPORTANT: this script should be DISABLED by default; it is ENABLED by logic in PlayerInteraction
public class ShadowPuzzleManager : MonoBehaviour
{
    private bool isPlaying = false; // if the player is playing the shadow puzzle or not

    private float rotation_speed = 50f; // sensitivity: 1f is good for camera, 50f is good for arrow keys
    private PlayerCam cam_script; // reference to camera controller
    private PlayerMovement movement_script; // reference to player movement

    // irregular objs will rotate strangely
    // use this anchor point to ensure they rotate around the correct point
    [SerializeField] Transform rotation_anchor_point;
    [SerializeField] Transform camera_anchor_point;
    [SerializeField] Camera main_cam;
    [SerializeField] GameObject player_camera_root;
    [SerializeField] TextMeshProUGUI instructions; // make sure this is disabled on the instruction UI (DON'T disable the UI GameObject itself)

    [Header("the object TO compare to")]
    [SerializeField] GameObject o2; 
    private float margin_of_error = 10f; // this seems to work alright

    private void Awake()
    {
        cam_script = FindObjectOfType<PlayerCam>();
        movement_script = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TogglePuzzle();
            this.enabled = false;
        }

        if (isPlaying)
        {
            RotateObj();

            if (DoesRotationMatch(transform.rotation, o2.transform.rotation))
            {
                Debug.Log("rotation matches");
            }
        }
    }

    private bool DoesRotationMatch(Quaternion rotation1, Quaternion rotation2)
    {
        float diff = Quaternion.Angle(rotation1, rotation2); // difference in angles
        return diff < margin_of_error;
    }

    // provides three axes of rotation
    private void RotateObj()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool plus = (Input.GetKey(KeyCode.Equals)); // + and = are usually the same key 
        bool minus = (Input.GetKey(KeyCode.Minus));

        float sensitivity = rotation_speed * Time.deltaTime;

        float rotation_x = h * sensitivity; 
        float rotation_y = v * sensitivity;

        transform.RotateAround(rotation_anchor_point.position, Vector3.up, -rotation_x);
        transform.RotateAround(rotation_anchor_point.position, Vector3.forward, -rotation_y);

        if (plus)
        {
            transform.RotateAround(rotation_anchor_point.position, Vector3.right, sensitivity);
        } else if (minus)
        {
            transform.RotateAround(rotation_anchor_point.position, Vector3.right, -sensitivity);
        }
    }

    // lock and unlock player control over movement and camera
    private void TogglePlayerControls()
    {
        cam_script.enabled = !cam_script.enabled;
        movement_script.enabled = !movement_script.enabled;
    }

    // use to enter and exit the puzzle (intend to use with player interaction raycast)
    public void TogglePuzzle()
    {
        TogglePlayerControls();

        isPlaying = !isPlaying;

        instructions.enabled = !instructions.enabled;

        if (isPlaying)
        {
            main_cam.transform.position = camera_anchor_point.transform.position;
            main_cam.transform.rotation = camera_anchor_point.transform.rotation;
        }
        else
        {
            main_cam.transform.position = player_camera_root.transform.position;
        }
    }
}
