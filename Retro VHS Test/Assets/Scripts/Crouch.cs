using UnityEngine;
using UnityEngine.InputSystem;

public class Crouch : MonoBehaviour
{
    public CharacterController PlayerHeight;
    public float normalHeight, crouchHeight;
    public float normalSpeed = 6f, crouchSpeed = 3f;

    public bool isCrouching; // Track if the player is crouching
    private float currentSpeed; // Current movement speed

    private PlayerInput playerInput; // Player input system
    private InputAction crouchAction; // Crouch action input
    private Vector2 moveInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        crouchAction = playerInput.actions["Crouch"]; // Ensure "Crouch" action is set in the Input Actions asset
        currentSpeed = normalSpeed; // Start with normal speed
    }

    void Update()
    {
        // Toggle crouch when the crouch action is triggered
        if (crouchAction.triggered)
        {
            if (isCrouching)
            {
                PlayerHeight.height = normalHeight;
                currentSpeed = normalSpeed;
                isCrouching = false;
            }
            else
            {
                PlayerHeight.height = crouchHeight;
                currentSpeed = crouchSpeed;
                isCrouching = true;
            }
        }

        // Get movement input and move the player
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Move player with adjusted speed
        PlayerHeight.Move(move * currentSpeed * Time.deltaTime);
    }

    // Return if the player is crouching
    public bool IsCrouching()
    {
        return isCrouching;
    }
}
