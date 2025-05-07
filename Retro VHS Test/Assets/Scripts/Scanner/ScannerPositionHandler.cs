using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerPositionHandler : MonoBehaviour
{
    [Header("Scanner Settings")]
    public Transform playerCamera;
    public LayerMask wallLayer;
    public float maxDistance = 0.5f;
    public float minDistance = 0.1f;
    public float smoothingSpeed = 10f;

    [Header("Default Position Settings")]
    private Vector3 defaultLocalPosition;
    private Quaternion defaultLocalRotation;

    [Header("Bobbing Settings")]
    public float idleBobFrequency = 2f;
    public float idleBobAmplitude = 0.04f;
    public float idleRotationAmplitude = 2f;

    public float moveBobFrequency = 8f;
    public float moveBobAmplitude = 0.1f;
    public float moveRotationAmplitude = 6f;

    private bool isFootstepBobbing = false;
    private float footstepBobbingIntensity = 0.1f;

    private float randomSideOffset = 0f;
    private float randomSideSpeed = 0.1f;
    private bool isRandomSideBobbing = false;
    private float randomBobbingTime = 0f;
    private float randomBobbingDuration = 0.5f;



    private float bobTimer = 0f;
    public Vector3 bobOffset;
    private Quaternion rotationOffset;

    [Header("Optional References")]
    public PlayerMovement playerMovement;

    void Start()
    {
        defaultLocalPosition = transform.localPosition;
        defaultLocalRotation = transform.localRotation;

        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    void Update()
    {
        Vector3 desiredLocalPosition = defaultLocalPosition;
        Quaternion desiredLocalRotation = defaultLocalRotation;

        RaycastHit hit;

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, wallLayer))
        {
            float safeDistance = Mathf.Max(hit.distance - 0.05f, minDistance);
            desiredLocalPosition = new Vector3(defaultLocalPosition.x, defaultLocalPosition.y, safeDistance);
        }

        bool isMoving = playerMovement != null && playerMovement.IsMoving();

        float frequency = isMoving ? moveBobFrequency : idleBobFrequency;
        float amplitude = isMoving ? moveBobAmplitude : idleBobAmplitude;
        float rotAmplitude = isMoving ? moveRotationAmplitude : idleRotationAmplitude;

        if (isFootstepBobbing)
        {
            amplitude += footstepBobbingIntensity;
            rotAmplitude += footstepBobbingIntensity;
            isFootstepBobbing = false;
        }

        if (isRandomSideBobbing)
        {
            randomBobbingTime += Time.deltaTime;
            float randomHorizontalBob = Mathf.Sin(randomBobbingTime * 10f) * randomSideOffset;

            randomSideOffset = Mathf.Lerp(randomSideOffset, 0f, randomSideSpeed * Time.deltaTime);

            desiredLocalPosition.x += randomHorizontalBob;

            if (randomBobbingTime > randomBobbingDuration)
            {
                isRandomSideBobbing = false;
            }
        }

        bobTimer += Time.deltaTime * frequency;

        float verticalBob = Mathf.Sin(bobTimer) * amplitude;
        float horizontalBob = Mathf.Cos(bobTimer * 0.5f) * amplitude;
        bobOffset = new Vector3(horizontalBob, verticalBob, 0f);

        float pitch = Mathf.Sin(bobTimer * 0.5f) * rotAmplitude;
        float yaw = Mathf.Cos(bobTimer) * rotAmplitude;
        rotationOffset = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 finalPosition = desiredLocalPosition + bobOffset;
        Quaternion finalRotation = defaultLocalRotation * rotationOffset;

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, smoothingSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation, smoothingSpeed * Time.deltaTime);
    }

    public void OnFootstep()
    {
        isFootstepBobbing = true;
        randomBobbingTime = 0f;
        isRandomSideBobbing = true;

        randomSideOffset = Random.Range(-0.1f, 0.1f);
    }
}
