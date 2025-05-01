using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Keypad : MonoBehaviour
{
    public GameObject player;
    public GameObject playerCam;
    public GameObject keypadUI;
    public GameObject scanner;
    public GameObject levelLoader;

    public TMP_Text textObj;

    public string answer = "1234";

    //public AudioSource button;
    //public AudioSource correct;
    //public AudioSource wrong;

    IEnumerator Start()
    {
        keypadUI.SetActive(false);
        
        yield return new WaitForSeconds(5);
        levelLoader.SetActive(false);
    }

    public void Number(int number)
    {
        textObj.text += number.ToString();
        //button.Play();
    }

    public void Execute()
    {
        if (textObj.text == answer)
        {
            //correct.Play();
            textObj.text = "Right";
            levelLoader.SetActive(true);
        }
        else
        {
            //wrong.Play();
            textObj.text = "Wrong";
            levelLoader.SetActive(false);
        }
    }

    public void Clear()
    {
        textObj.text = "";
        //button.Play();
    }

    public void Exit()
    {
        keypadUI.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;
        playerCam.GetComponent<MouseLook>().enabled = true;
        scanner.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            keypadUI.SetActive(true);
        }

        if (keypadUI.activeInHierarchy)
        {
            player.GetComponent<PlayerMovement>().enabled = false;
            playerCam.GetComponent<MouseLook>().enabled = false;
            scanner.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }        
    }
}
