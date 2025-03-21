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
    public float walkSpeed = 12f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    Vector2 moveInput;
    private float speed;
    private bool isGrounded;

    private PlayerInput playerInput;
    private bool isJumping;

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
        playerInput = GetComponent<PlayerInput>();

        breathingSource.clip = breathingSound;
        breathingSource.loop = true;
        breathingSource.volume = 0.2f;
        breathingSource.Play();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        isMoving = move.magnitude > 0.1f && isGrounded;

        controller.Move(move * speed * Time.deltaTime);

        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
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

    public void OnJump(InputValue value)
    {
        isJumping = value.isPressed;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
