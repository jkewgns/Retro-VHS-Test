using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

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

    //Bobbing Variables
    [Header("View Bobbing")]
    public float bobFrequency = 8f;
    public float bobAmplitude = 0.05f;
    private float bobTimer = 0f;
    public float bobHorizontalAmplitude = 0.03f;

    public ScannerPositionHandler scannerPositionHandler;

    // Health Variables
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    // Audio Variables
    [Header("Audio Settings")]
    public AudioClip[] footstepSounds;
    public AudioClip[] waterFootstepSounds;
    public AudioClip breathingSound;
    public AudioClip landingSound;
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

    // Camera Shake Variables
    [Header("Camera Shake")]
    public Transform cameraTransform;
    private Vector3 originalCameraPosition;
    private Vector3 shakeOffset = Vector3.zero;
    private Vector3 bobOffset = Vector3.zero;
    private bool isShaking;

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

        originalCameraPosition = cameraTransform.localPosition;
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
        HandleViewBobbing();
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

                    if (scannerPositionHandler != null)
                    {
                        scannerPositionHandler.OnFootstep();
                    }
                }
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void HandleViewBobbing()
    {
        if (isMoving && isGrounded)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            
            float verticalBob = Mathf.Sin(bobTimer) * bobAmplitude;
            float horizontalBob = Mathf.Sin(bobTimer * 0.5f) * bobHorizontalAmplitude * moveInput.x;

            bobOffset = new Vector3(horizontalBob, verticalBob, 0);
        }
        else
        {
            bobTimer = 0f;
            bobOffset = Vector3.Lerp(bobOffset, Vector3.zero, Time.deltaTime * 10f);
        }

        ApplyCameraOffset();
    }

    void CheckLanding()
    {
        if (!isGrounded) 
        {
            lastYPosition = Mathf.Max(lastYPosition, transform.position.y);
        }
        else 
        {
            float fallDistance = lastYPosition - transform.position.y;

            if (fallDistance > 3f) 
            {
                PlayLandingSound(fallDistance);
                StartCoroutine(CameraShake(fallDistance));
            }
            lastYPosition = transform.position.y;
        }
    }

    void PlayLandingSound(float fallDistance)
    {
        if (landingSound != null)
        {
            float volume;

            if (fallDistance >= 18f)
                volume = 1f;
            else if (fallDistance >= 12f)
                volume = 0.7f;
            else if (fallDistance >= 8f)
                volume = 0.4f;
            else
                volume = 0.2f;

            audioSource.PlayOneShot(landingSound, volume);
        }
    }

    IEnumerator CameraShake(float fallDistance)
    {
        if (isShaking) yield break;
        isShaking = true;

        float shakeDuration;
        float shakeIntensity;

        if (fallDistance >= 18f)
        {
            shakeDuration = 0.5f;
            shakeIntensity = 0.3f;
        }
        else if (fallDistance >= 12f)
        {
            shakeDuration = 0.35f;
            shakeIntensity = 0.2f;
        }
        else if (fallDistance >= 8f)
        {
            shakeDuration = 0.2f;
            shakeIntensity = 0.1f;
        }
        else
        {
            shakeDuration = 0.1f;
            shakeIntensity = 0.05f;
        }

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            shakeOffset = new Vector3(x, y, 0);
            ApplyCameraOffset();

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        ApplyCameraOffset();
        isShaking = false;
    }

    public bool IsCrouching()
    {
        return isCrouching;
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

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
    }

    public void Respawn()
    {
        transform.position = startingPosition;
        currentHealth = maxHealth;
    }

    void ApplyCameraOffset()
    {
        cameraTransform.localPosition = originalCameraPosition + bobOffset + shakeOffset;
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

    public bool IsMoving()
    {
        return isMoving;
    }
}
