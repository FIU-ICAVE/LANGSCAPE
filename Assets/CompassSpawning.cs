using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassSpawning : MonoBehaviour
{

    //Gesture Test
    public GestureTest gesture;
    public GameObject compass;

    // Update is called once per frame
    void Update()
    {
        if (gesture.selected){
            Debug.Log("Hello");

            compass.SetActive(true);
        }
        else
        {
            compass.SetActive(false);
        }
        
    }
}
