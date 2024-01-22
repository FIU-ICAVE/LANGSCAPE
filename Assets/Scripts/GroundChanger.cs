using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChanger : MonoBehaviour
{

    //public TMPro.TMP_Dropdown GroundDropdown; // for the dropdown menu
    public GameObject[] GroundStates = new GameObject[7]; // Array of ground state game objects
    int area; // Stores Current Active State

    void Start() 
    {
        //GroundDropdown.onValueChanged.AddListener(delegate { changeGround(GroundDropdown.value); });
        GroundStates[0].SetActive(true); //sets the game start area, currently forest
        area = 1; //keeps track of the land for utility
    }
    
    public void changeGround(int groundType2) 
    {
        int groundType = groundType2 - 1;
        // Deactivates all ground states
        for(int i = 0; i < GroundStates.Length; i++) 
        {
            GroundStates[i].SetActive(false);
        }

        // Reactivates the chosen ground state
        if(groundType >= 0 && groundType <= GroundStates.Length) 
        {
            GroundStates[groundType].SetActive(true);
            area = groundType2;
        }
        else 
        {
            Debug.Log("Invalid ground type");
        }
    }
    public int currentGround()
    {
        return area;
    }
}
