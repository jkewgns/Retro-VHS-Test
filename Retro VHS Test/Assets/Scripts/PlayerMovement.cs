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
    public AudioClip[] waterFootstepSounds; // New array for water footstep sounds
    public AudioClip breathingSound;
    public AudioClip landingSound; // New landing sound
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

    // Water Detection
    [HideInInspector] public bool isInWater = false;

    private Vector3 startingPosition;
    private float lastYPosition;

    void Start()
    {
        startingPosition = transform.position;

        speed = walkSpeed;
        defaultStepInterval = stepInterval;
        playerInput = GetComponent<PlayerInput>();

        breathingSource.clip = breathingSound;
        breathingSource.loop = true;
        breathingSource.volume = 0.2f;
        breathingSource.Play();

        currentHealth = maxHealth;
        lastYPosition = transform.position.y;
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
        CheckLanding();
    }

    void PlayFootstepSFX()
    {
        if (isMoving)
        {
            stepTimer += Time.deltaTime;

            if (isInWater && isCrouching)
            {
                stepInterval = defaultStepInterval * 2f;
            }
            else if (isInWater)
            {
                stepInterval = defaultStepInterval; 
            }
            else if (isCrouching)
            {
                stepInterval = defaultStepInterval * 2f;
            }
            else
            {
                stepInterval = defaultStepInterval;
            }

            if (stepTimer >= stepInterval)
            {
                stepTimer = 0f;

                AudioClip[] currentFootstepSounds = isInWater ? waterFootstepSounds : footstepSounds;

                if (currentFootstepSounds.Length > 0)
                {
                    int randomIndex = Random.Range(0, currentFootstepSounds.Length);
                    audioSource.volume = isInWater ? 0.5f : 0.1f;
                    audioSource.PlayOneShot(currentFootstepSounds[randomIndex]);
                }
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void CheckLanding()
    {
        // Check if player has landed after falling from a significant height
        if (isGrounded && transform.position.y < lastYPosition - 1f) // Threshold for fall height
        {
            Debug.Log("Player landed after falling from a height");  // Debug log for landing detection
            PlayLandingSound();
        }

        lastYPosition = transform.position.y;
    }

    void PlayLandingSound()
    {
        if (landingSound != null)
        {
            audioSource.PlayOneShot(landingSound);
            Debug.Log("Landing sound played: " + landingSound.name);  // Debug log for landing SFX
        }
        else
        {
            Debug.LogWarning("Landing sound is not assigned!");  // Warning if landing sound is not assigned
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
        transform.position = startingPosition;
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
