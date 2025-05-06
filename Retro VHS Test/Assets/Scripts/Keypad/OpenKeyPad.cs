using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenKeyPad : MonoBehaviour
{
    public GameObject keypadObj;
    public GameObject keypadText;

    public Transform player;

    public float activationDistance = 3f;
    private bool isNearKeypad = false;

    // Start is called before the first frame update
    void Start()
    {
        isNearKeypad = false;
        keypadText.SetActive(false);
        keypadObj.GetComponent<Keypad>().enabled = false;
    }

/*
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            keypadText.SetActive(true);
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            keypadText.SetActive(false);
        }
    }
*/
    // Update is called once per frame
    void Update()
    {
/*
        if (Input.GetButtonDown("Interact")  && inReach)
        {
            keypadObj.SetActive(true);
        }
*/
        float distance = Vector3.Distance(player.position, keypadObj.transform.position);
        if (distance <= activationDistance)
        {
            if (!isNearKeypad)
            {
                isNearKeypad = true;
                keypadText.SetActive(true);
                keypadObj.GetComponent<Keypad>().enabled = true;
            }
        }
        else
        {
            if (isNearKeypad)
            {
                isNearKeypad = false;
                keypadText.SetActive(false);
                keypadObj.GetComponent<Keypad>().enabled = false;
                
            }
        }
    }
}
