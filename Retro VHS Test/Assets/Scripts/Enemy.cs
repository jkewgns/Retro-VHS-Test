using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 5f;
    public float detectionRange = 10f;
    public float initialMovementSpeed = 1f;
    public float maxMovementSpeed = 5f;
    public float speedIncreaseRate = 0.1f;

    private float currentMovementSpeed;
    private PlayerMovement playerMovement; // Reference to the PlayerMovement script
    private Crouch crouchScript; // Reference to the Crouch script

    private void Start()
    {
        // Initialize the enemy's movement speed
        currentMovementSpeed = initialMovementSpeed;

        // Get references to the player movement and crouch scripts
        playerMovement = player.GetComponent<PlayerMovement>();
        crouchScript = player.GetComponent<Crouch>();
    }

    private void Update()
    {
        // If player is crouching, stop enemy from moving or looking at the player
        if (crouchScript != null && crouchScript.IsCrouching())
        {
            return;
        }

        // Check if player is within detection range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            LookAtPlayer();
            MoveTowardsPlayer();
            IncreaseSpeedOverTime();
        }
    }

    // Rotate enemy to face the player
    private void LookAtPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // Move the enemy towards the player
    private void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.position += directionToPlayer * Time.deltaTime * currentMovementSpeed;
    }

    // Gradually increase the speed of the enemy over time
    private void IncreaseSpeedOverTime()
    {
        if (currentMovementSpeed < maxMovementSpeed)
        {
            currentMovementSpeed += speedIncreaseRate * Time.deltaTime;
        }
    }

    // Restart the scene when the player collides with the enemy
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
