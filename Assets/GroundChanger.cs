using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChanger : MonoBehaviour
{

    public TMPro.TMP_Dropdown GroundDropdown; // for the dropdown menu
    public GameObject[] GroundStates = new GameObject[6]; // Array of ground state game objects

    void Start() 
    {
        GroundDropdown.onValueChanged.AddListener(delegate { changeGround(GroundDropdown.value); });
        GroundStates[1].SetActive(true); //sets the game start area, currently forest
    }

    public void changeGround(int groundType) 
    {
        // Deactivates all ground states
        for(int i = 0; i < GroundStates.Length; i++) 
        {
            GroundStates[i].SetActive(false);
        }

        // Reactivates the chosen ground state
        if(groundType >= 0 && groundType <= GroundStates.Length) 
        {
            GroundStates[groundType].SetActive(true);
        }
        else 
        {
            Debug.Log("Invalid ground type");
        }
    }
}
