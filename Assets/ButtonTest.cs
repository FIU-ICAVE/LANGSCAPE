using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;


public class ButtonTest : MonoBehaviour
{
    public GameObject button;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int selected = button.GetComponent<InteractableColorVisual>().selected;
        if(selected == 1){
            Debug.Log("Fuck you");
        }
        
    }
}
