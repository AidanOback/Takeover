using UnityEngine;
using UnityEngine.InputSystem;

public class GhostMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera ghostCamera;
    [SerializeField] private CharacterController controller;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 0.15f;

    private float pitch;
    private bool ghostEnabled;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        ghostEnabled = false;

        if (controller != null)
            controller.enabled = false;

        if (ghostCamera != null)
            ghostCamera.enabled = false;
    }

    private void Update()
    {
        if (!ghostEnabled)
            return;

        HandleLook();
        HandleMovement();
    }

    public void SetGhostEnabled(bool enabled)
    {
        ghostEnabled = enabled;

        if (controller != null)
            controller.enabled = enabled;

        if (ghostCamera != null)
            ghostCamera.enabled = enabled;

        if (enabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void HandleMovement()
    {
        if (Keyboard.current == null)
            return;

        Vector3 movement = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
            movement += transform.forward;

        if (Keyboard.current.sKey.isPressed)
            movement -= transform.forward;

        if (Keyboard.current.dKey.isPressed)
            movement += transform.right;

        if (Keyboard.current.aKey.isPressed)
            movement -= transform.right;

        if (Keyboard.current.spaceKey.isPressed)
            movement += Vector3.up;

        if (Keyboard.current.leftCtrlKey.isPressed)
            movement -= Vector3.up;

        movement.Normalize();

        controller.Move(
            movement * moveSpeed * Time.deltaTime
        );
    }

    private void HandleLook()
    {
        if (Mouse.current == null || ghostCamera == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float yaw =
            mouseDelta.x * mouseSensitivity;

        float lookPitch =
            mouseDelta.y * mouseSensitivity;

        transform.Rotate(Vector3.up * yaw);

        pitch -= lookPitch;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        ghostCamera.transform.localRotation =
            Quaternion.Euler(pitch, 0f, 0f);
    }
}