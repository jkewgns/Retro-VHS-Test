using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingEnemy : MonoBehaviour
{
    public GameObject enemy;

    void Start()
    {
        enemy.SetActive(false);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            enemy.SetActive(true);
        }

        if(other.CompareTag("Enemy"))
        {
            enemy.SetActive(false);
        }
    }
}
