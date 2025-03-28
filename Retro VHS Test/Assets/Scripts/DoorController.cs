using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator;
    public float speed = 1f;  // Normal speed of 1.0 (you can change this dynamically)

    void Start()
    {
        doorAnimator.speed = speed;  // Set the speed at runtime
    }

    public void SetDoorSpeed(float newSpeed)
    {
        doorAnimator.speed = newSpeed;  // Change speed dynamically
    }
}
