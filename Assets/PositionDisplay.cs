//
// Authors: Christian Laverde
//
// Description: Generates values to be input into the Wrist UI Canvas;
// player position is determined by taking the x, y, z coordinates of the OVRInteraction GameObject,
// the position the teleport will send you to is taken from the TeleportInteractor script from the
// GameObject TeleportHandInteractor. These values are formatted into integers and displayed.
//


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Oculus.Interaction.Locomotion;

public class PositionDisplay : MonoBehaviour
{

    public TextMeshProUGUI positionText;
    public TextMeshProUGUI teleportPositionText;
    public TeleportInteractor teleportScript;
    

    void Update()
    {
        //Get target teleport position from gameObject TeleportHandInteractor
        Vector3 teleportPosition = teleportScript.TeleportTarget.position;

        //Get the position of this GameObject, format as int so that it chops off the decimal point
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        int z = (int)transform.position.z;

        //Translate 3D Vector teleport position into int format
        int nextX = (int)teleportPosition.x;
        int nextY = (int)teleportPosition.y;
        int nextZ = (int)teleportPosition.z;

        //Format values to be visually coherent
        string position = string.Format("{0}, {1}, {2}", x, y, z);
        string nextPosition = string.Format("{0}, {1}, {2}", nextX, nextY, nextZ);



        //Update values for UI
        positionText.text = position;
        teleportPositionText.text = nextPosition;
    }
}
