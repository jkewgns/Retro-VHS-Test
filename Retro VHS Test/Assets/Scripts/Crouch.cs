using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crouch : MonoBehaviour
{
    public CharacterController PlayerHeight;
    public float normalHeight, crouchHeight;
    public float normalSpeed = 6f, crouchSpeed = 3f;

    private bool isCrouching;
    private float currentSpeed;

    private PlayerInput playerInput;
    private InputAction crouchAction;
    private Vector2 moveInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        crouchAction = playerInput.actions["Crouch"]; // Ensure "Crouch" action is set in the Input Actions asset
        currentSpeed = normalSpeed; // Start with normal speed (6f)
    }

    void Update()
    {
        // Toggle crouch
        if (crouchAction.triggered)
        {
            if (isCrouching)
            {
                PlayerHeight.height = normalHeight;
                currentSpeed = normalSpeed; // Set speed back to normal (6f)
                isCrouching = false;
            }
            else
            {
                PlayerHeight.height = crouchHeight;
                currentSpeed = crouchSpeed; // Set speed to crouch speed (3f)
                isCrouching = true;
            }
        }

        // Get movement input and move player
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Use the `currentSpeed` when moving the player
        PlayerHeight.Move(move * currentSpeed * Time.deltaTime);
    }
}
