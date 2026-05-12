using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Camera")]
    public Camera    cam;
    public Transform cameraTarget;
    public float     mouseSensitivity = 2f;
    public float     cameraDistance   = 5f;
    public float     cameraMinY       = -20f;
    public float     cameraMaxY       = 60f;

    // Set this from NPCSystems.cs to lock/unlock the player
    static public bool dialouge = false;

    public static ThirdPersonController Instance { get; private set; }

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity   = -20f;

    CharacterController _cc;
    float _rotX;
    float _rotY;
    float _velY;
    bool  _altMode;
    bool  _wasInDialouge = false; // tracks previous frame's state

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void Update()
    {
        // --- Handle dialogue lock/unlock transitions ---
        if (dialouge != _wasInDialouge)
        {
            if (dialouge)
            {
                // Entering dialogue: free the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible   = true;
                _altMode         = false; // cancel alt mode if it was active
            }
            else
            {
                // Leaving dialogue: re-lock the cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible   = false;
            }
            _wasInDialouge = dialouge;
        }

        // --- Block all input while in dialogue ---
        if (dialouge) return;

        // Alt toggle (only active outside dialogue)
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            _altMode         = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            _altMode         = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        if (_altMode) return;

        // Camera rotation
        _rotY += Input.GetAxis("Mouse X") * mouseSensitivity;
        _rotX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        _rotX  = Mathf.Clamp(_rotX, cameraMinY, cameraMaxY);

        // Movement
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = cam.transform.forward;
        Vector3 right   = cam.transform.right;
        forward.y = 0;
        right.y   = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * v + right * h).normalized;

        if (_cc.isGrounded)
            _velY = -2f;
        else
            _velY += gravity * Time.deltaTime;

        move.y = _velY;
        _cc.Move(move * moveSpeed * Time.deltaTime);

        Vector3 flatMove = new Vector3(move.x, 0, move.z);
        if (flatMove.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(flatMove),
                15f * Time.deltaTime
            );
        }
    }

    void LateUpdate()
    {
        // Camera still follows the player during dialogue (it just can't rotate)
        if (cam == null) return;

        Vector3 pivot = cameraTarget != null
            ? cameraTarget.position
            : transform.position + Vector3.up * 1.5f;

        Quaternion rot         = Quaternion.Euler(_rotX, _rotY, 0);
        cam.transform.position = pivot + rot * new Vector3(0, 0, -cameraDistance);
        cam.transform.LookAt(pivot);
    }
}