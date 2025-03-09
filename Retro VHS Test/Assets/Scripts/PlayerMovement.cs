using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float runSpeed = 20f;
    public float walkSpeed = 12f;
    public float crouchSpeed = 6f;

    public float standHeight = 2f;
    public float crouchHeight = 0.2f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    Vector2 moveInput;
    private float speed;
    private bool isGrounded;

    public float maxStamina = 100f;
    private float currentStamina;
    public float staminaDrainRate = 10f;
    public float staminaRegenRate = 5f;
    private bool isSprinting;

    public Slider staminaSlider;
    public GameObject staminaUI;

    private PlayerInput playerInput;
    private bool isJumping;
    private bool isCrouching;

    public AudioClip[] footstepSounds;
    public AudioClip breathingSound;
    public AudioSource audioSource;
    public AudioSource breathingSource;
    
    private bool isMoving;
    private float stepTimer = 0f;
    public float stepInterval = 0.5f;

    void Start()
    {
        speed = walkSpeed;
        currentStamina = maxStamina;
        playerInput = GetComponent<PlayerInput>();

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        staminaUI.SetActive(false);

        breathingSource.clip = breathingSound;
        breathingSource.loop = true;
        breathingSource.volume = 0.2f;
        breathingSource.Play();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        isMoving = move.magnitude > 0.1f && isGrounded;

        controller.Move(move * speed * Time.deltaTime);

        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (isSprinting && currentStamina > 0)
        {
            speed = runSpeed;
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (!staminaUI.activeSelf)
            {
                staminaUI.SetActive(true);
            }
        }
        else
        {
            isSprinting = false;

            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }

            speed = walkSpeed;

            if (staminaUI.activeSelf)
            {
                staminaUI.SetActive(false);
            }
        }

        if (isCrouching)
        {
            controller.height = crouchHeight;
            speed = crouchSpeed;
        }
        else if (!isSprinting)
        {
            controller.height = standHeight;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }

        PlayFootstepSFX();
    }

    void PlayFootstepSFX()
    {
        if (isMoving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                stepTimer = 0f;
                if (footstepSounds.Length > 0)
                {
                    int randomIndex = Random.Range(0, footstepSounds.Length);
                    audioSource.PlayOneShot(footstepSounds[randomIndex]);
                }
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        isJumping = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed && currentStamina > 0;
    }

    public void OnCrouch(InputValue value)
    {
        isCrouching = value.isPressed;
    }
}
