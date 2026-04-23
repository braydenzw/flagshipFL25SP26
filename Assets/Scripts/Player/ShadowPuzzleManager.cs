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

    [Header("The object TO compare to")]
    [SerializeField] GameObject o2; 
    private float margin_of_error = 10f; // exact match tolerance
    
    // --- NEW SPOTLIGHT VARIABLES ---
    [Header("Spotlight Settings")]
    [SerializeField] private Light puzzleSpotlight; // Drag your spotlight here in the inspector
    [SerializeField] private float close_margin_of_error = 45f; // Tolerance for turning yellow
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color closeColor = Color.yellow;
    [SerializeField] private Color correctColor = Color.green;
    // -------------------------------

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
            CheckShadowMatch(); // Call the new method here
        }
    }

    // --- NEW METHOD FOR CHECKING MATCH AND COLORS ---
    private void CheckShadowMatch()
    {
        // Calculate difference in angles just once per frame
        float diff = Quaternion.Angle(transform.rotation, o2.transform.rotation); 

        if (diff <= margin_of_error)
        {
            puzzleSpotlight.color = correctColor;
            Debug.Log("rotation matches");
            
            // You can also trigger your "Puzzle Solved" logic here!
        }
        else if (diff <= close_margin_of_error)
        {
            puzzleSpotlight.color = closeColor;
        }
        else
        {
            puzzleSpotlight.color = defaultColor;
        }
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