using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureTest : MonoBehaviour
{
    //Just a tracker of whether thumb is up or down
 
    [HideInInspector] public bool selected = false;

    public void Select(){
        selected = !selected;
    }

}
