using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCollapse : MonoBehaviour
{
    public Rigidbody rb;
    public AudioSource smashSFX;

    void Start()
    {
        rb.useGravity = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            rb.useGravity = true;
            StartCoroutine(BridgeFall());
        }
    }

    IEnumerator BridgeFall()
    {
        yield return new WaitForSeconds(1);

        smashSFX.Play();

        Destroy(this);
    }
}
