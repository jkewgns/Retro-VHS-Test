using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dnd : MonoBehaviour
{
    private static GameObject DNDObject;

    // Start is called before the first frame update
    void Awake()
    {
        DND();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Do not destroy on loading a new scene
    void DND()
    {
        DontDestroyOnLoad(this.gameObject);
        
        if (DNDObject == null) 
        {
            DNDObject = this.gameObject;
        } 
        else 
        {
            Destroy(this.gameObject);
        }
    }
}
