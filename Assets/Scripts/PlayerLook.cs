using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode; // Required for multiplayer

// Change from MonoBehaviour to NetworkBehaviour
public class PlayerLook : NetworkBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 0.2f;
    public Transform playerBody;
    
    [Header("Input Bindings")]
    public InputActionReference lookAction;

    private float xRotation = 0f;
    private Camera playerCamera;
    private AudioListener audioListener;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        audioListener = GetComponent<AudioListener>();
    }

    // This runs the moment the server spawns the player prefab
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            // Turn off the camera and ears if this is someone else's body
            playerCamera.enabled = false;
            audioListener.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (lookAction != null) lookAction.action.Enable();
    }

    private void OnDisable()
    {
        if (lookAction != null) lookAction.action.Disable();
    }

    private void Start()
    {
        // Only lock the mouse if we actually own this character
        if (IsOwner) Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Ignore input if we do not own this character
        if (!IsOwner) return;

        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}