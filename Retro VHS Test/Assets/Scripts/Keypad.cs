using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Keypad : MonoBehaviour
{
    public GameObject player;
    public GameObject keypadObj;

    public TMP_Text textObj;
    public string answer = "1234";

    public AudioSource button;
    public AudioSource correct;
    public AudioSource wrong;

    void Start()
    {
        keypadObj.SetActive(false);
    }

    public void Number(int number)
    {
        textObj.text += number.ToString();
        button.Play();
    }

    public void Execute()
    {
        if (textObj.text == answer)
        {
            correct.Play();
            textObj.text = "Right";
        }
        else
        {
            wrong.Play();
            textObj.text = "Wrong";
        }
    }

    public void Clear()
    {
        textObj.text = "";
        button.Play();
    }

    public void Exit()
    {
        keypadObj.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;
    }

    public void Update()
    {
        if (keypadObj.activeInHierarchy)
        {
            player.GetComponent<PlayerMovement>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }        
    }
}
