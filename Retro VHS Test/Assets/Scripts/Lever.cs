using UnityEngine;

public class Lever : MonoBehaviour
{
    public Transform player;
    public Animator doorAnimator; // Door1's Animator
    public Animator lever2Animator; // Lever2's Animator
    public GameObject promptUI;
    public float activationDistance = 5f;

    private bool isNearLever = false;
    private bool isOpen = false;

    private void Start()
    {
        // Optionally, you can reset the lever2 animation to its default state
        lever2Animator.SetTrigger("Pull"); // Reset or start Lever2 in neutral state
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= activationDistance)
        {
            if (!isNearLever)
            {
                isNearLever = true;
                promptUI.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
        else
        {
            if (isNearLever)
            {
                isNearLever = false;
                promptUI.SetActive(false);
            }
        }
    }

    private void Interact()
    {
        // Toggle Door1 (open/close)
        isOpen = !isOpen;
        doorAnimator.SetBool("Open", isOpen);

        // Play Lever2's open/close animations based on the state of Door1
        if (isOpen)
        {
            // Door1 is opening, play Lever2's open animation
            lever2Animator.SetTrigger("Pull"); // Trigger Lever_Open animation for Lever2
        }
        else
        {
            // Door1 is closing, play Lever2's close animation
            lever2Animator.SetTrigger("Pull"); // Trigger Lever_Close animation for Lever2
        }
    }
}
