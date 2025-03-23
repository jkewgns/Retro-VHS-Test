using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Movement Settings")]
    public float rotationSpeed = 5f;
    public float detectionRange = 10f;
    public float initialMovementSpeed = 1f;
    public float maxMovementSpeed = 5f;
    public float speedIncreaseRate = 0.1f;

    [Header("References")]
    public Transform player;
    public LayerMask obstacleLayer;
    public AudioSource audioSource;

    private float currentMovementSpeed;
    private PlayerMovement playerMovement;

    private void Start()
    {
        currentMovementSpeed = initialMovementSpeed;
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement != null && playerMovement.IsCrouching())
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            LookAtPlayer();
            MoveTowardsPlayer();
            IncreaseSpeedOverTime();
        }
    }

    private void LookAtPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        if (directionToPlayer.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, 1f, obstacleLayer))
        {
            return;
        }

        transform.position += directionToPlayer * Time.deltaTime * currentMovementSpeed;
    }

    private void IncreaseSpeedOverTime()
    {
        if (currentMovementSpeed < maxMovementSpeed)
        {
            currentMovementSpeed += speedIncreaseRate * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadCurrentLevel();
        }
    }

    public void LoadCurrentLevel()
    {
        if (audioSource != null)
        {
            audioSource.volume = 1f;
        }

        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(levelIndex);
    }
}
