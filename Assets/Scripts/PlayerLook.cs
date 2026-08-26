using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

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

    private bool canLook = true;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        audioListener = GetComponent<AudioListener>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerCamera.enabled = false;
            audioListener.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (lookAction != null)
        {
            lookAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (lookAction != null)
        {
            lookAction.action.Disable();
        }
    }

    private void Start()
    {
        if (IsOwner)
        {
            LockCursor();
        }
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (!canLook)
            return;

        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(
            Vector3.up * mouseX
        );
    }

    //Face Editor

    public void SetLookEnabled(bool enabled)
    {
        if (!IsOwner)
            return;

        canLook = enabled;

        if (enabled)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}