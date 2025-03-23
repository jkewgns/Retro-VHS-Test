using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Movement Variables
    [Header("Movement Settings")]
    public CharacterController controller;
    public float gravity = -9.81f;
    public float gravityMultiplier = 2f;
    public float walkSpeed = 12f;
    public float crouchSpeed = 6f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // Health Variables
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    // Audio Variables
    [Header("Audio Settings")]
    public AudioClip[] footstepSounds;
    public AudioClip breathingSound;
    public AudioSource audioSource;
    public AudioSource breathingSource;

    // Footstep Settings
    private float stepTimer = 0f;
    public float stepInterval = 0.5f;
    private float defaultStepInterval;

    // Movement Variables
    private Vector3 velocity;
    private Vector2 moveInput;
    private float speed;
    private bool isGrounded;
    private bool isCrouching;

    private PlayerInput playerInput;
    private bool isMoving;

    // Store initial position for respawn
    private Vector3 startingPosition;

    void Start()
    {
        // Store the starting position of the player
        startingPosition = transform.position;

        speed = walkSpeed;
        defaultStepInterval = stepInterval;
        playerInput = GetComponent<PlayerInput>();

        breathingSource.clip = breathingSound;
        breathingSource.loop = true;
        breathingSource.volume = 0.2f;
        breathingSource.Play();

        currentHealth = maxHealth; // Set initial health
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        isMoving = move.magnitude > 0.1f && isGrounded;

        controller.Move(move * speed * Time.deltaTime);

        if (!isGrounded)
            velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        else
            velocity.y = 0f;

        controller.Move(velocity * Time.deltaTime);

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
                    audioSource.volume = 0.1f;
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

    public void OnCrouch()
    {
        isCrouching = !isCrouching;
        speed = isCrouching ? crouchSpeed : walkSpeed;
        stepInterval = isCrouching ? defaultStepInterval * 2f : defaultStepInterval;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Player Health: " + currentHealth);
    }

    public void Respawn()
    {
        Debug.Log("Player has died! Respawning at starting position...");
        transform.position = startingPosition; // Teleport the player back to the starting position
        currentHealth = maxHealth; // Reset health to max
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
