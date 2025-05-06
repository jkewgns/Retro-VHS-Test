using UnityEngine;

public class HiddenLever : MonoBehaviour
{
    public Transform player;
    public Animator doorAnimator;
    public GameObject promptUI;
    public float activationDistance = 5f;

    private bool isNearLever = false;
    private bool isOpen = false;

    private void Start()
    {
        // Initial setup for Lever2 if needed
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
        // Toggle the door (Door1) open/close
        isOpen = !isOpen;
        doorAnimator.SetBool("Open", isOpen);
    }
}
