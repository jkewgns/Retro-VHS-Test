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

    void Start()
    {
        defaultLocalPosition = transform.localPosition;
        defaultLocalRotation = transform.localRotation;
    }

    void Update()
    {
        Vector3 desiredLocalPosition = defaultLocalPosition;
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, wallLayer))
        {
            float safeDistance = Mathf.Max(hit.distance - 0.05f, minDistance);
            desiredLocalPosition = new Vector3(defaultLocalPosition.x, defaultLocalPosition.y, safeDistance);
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredLocalPosition, smoothingSpeed * Time.deltaTime);
        transform.localRotation = defaultLocalRotation;
    }
}
