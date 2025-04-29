using UnityEngine;

public class Lever : MonoBehaviour
{
    public Transform player;
    public Animator doorAnimator;
    public Animator lever2Animator;
    public GameObject promptUI;
    public float activationDistance = 5f;

    private bool isNearLever = false;
    private bool isOpen = false;

    private void Start()
    {
        lever2Animator.SetTrigger("Pull");
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
        isOpen = !isOpen;
        doorAnimator.SetBool("Open", isOpen);

        if (isOpen)
        {
            lever2Animator.SetTrigger("Pull");
        }
        else
        {
            lever2Animator.SetTrigger("Pull");
        }
    }
}
