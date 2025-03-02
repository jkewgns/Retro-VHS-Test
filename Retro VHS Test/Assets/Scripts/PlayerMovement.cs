using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import the UI namespace

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float runSpeed = 12f;
    public float walkSpeed = 6f;
    public float crouchSpeed = 3f;

    public float standHeight = 2f;
    public float crouchHeight = 0.2f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    private float speed;
    private bool isGrounded;

    public float maxStamina = 100f;
    private float currentStamina;
    public float staminaDrainRate = 10f;
    public float staminaRegenRate = 5f;
    private bool isSprinting;

    public Slider staminaSlider;
    public GameObject staminaUI;

    // Start is called before the first frame update
    void Start()
    {
        speed = walkSpeed;
        currentStamina = maxStamina;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        staminaUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl) && currentStamina > 0)
        {
            isSprinting = true;
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

        // Crouch
        if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.height = crouchHeight;
            speed = crouchSpeed;
        }
        // Normal Walk
        else if (!isSprinting)
        {
            controller.height = standHeight;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }
}