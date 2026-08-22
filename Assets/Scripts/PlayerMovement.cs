using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode; // 1. Add the Netcode namespace

[RequireComponent(typeof(CharacterController))]
// 2. Change MonoBehaviour to NetworkBehaviour
public class PlayerMovement : NetworkBehaviour 
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 12f;
    public float gravity = -19.62f;
    public float jumpHeight = 1.5f;

    [Header("Momentum Settings")]
    public float movementSmoothTime = 0.1f; 
    public float speedSmoothTime = 0.15f;

    [Header("Input Bindings")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;
    private float currentSpeed;
    private float speedSmoothVelocity;

    private void Awake() => controller = GetComponent<CharacterController>();

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (jumpAction != null) jumpAction.action.Enable();
        if (sprintAction != null) sprintAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
        if (jumpAction != null) jumpAction.action.Disable();
        if (sprintAction != null) sprintAction.action.Disable();
    }

    private void Update()
    {
        // 3. The Magic Network Check: If this isn't our player, ignore the rest of the code!
        if (!IsOwner) return;

        // Ground Check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f; 

        // Speed & Sprinting
        float targetSpeed = walkSpeed;
        if (sprintAction != null && sprintAction.action.IsPressed()) targetSpeed = sprintSpeed;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        // Smooth Movement
        Vector2 targetInput = moveAction.action.ReadValue<Vector2>();
        currentInputVector = Vector2.SmoothDamp(currentInputVector, targetInput, ref smoothInputVelocity, movementSmoothTime);
        Vector3 move = transform.right * currentInputVector.x + transform.forward * currentInputVector.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jumping
        if (jumpAction != null && jumpAction.action.IsPressed() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}