/*
    Author: Ian Rodriguez, Herbert Cabrera
    Script Description:
        This script defines the grid system to which blocks are placed on.
    
    TO DO:
*/
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    private float _pointDensity = 1.0f; //no space between blocks

    public Vector3 GetNearestPointOnGrid(Vector3 pos)
    {
        pos -= transform.position; //for making sure points move if grid is moved

        //getting number of points across grid
        int xCount = Mathf.RoundToInt(pos.x / _pointDensity);
        int yCount = Mathf.RoundToInt(pos.y / _pointDensity) ;
        int zCount = Mathf.RoundToInt(pos.z / _pointDensity);

        //adjusting according to "size"
        Vector3 result = new Vector3(
            (float)xCount * _pointDensity + (float)1/2,
            (float)yCount * _pointDensity + (float)1/2,
            (float)zCount * _pointDensity + (float)1/2);

        result += transform.position; //for making sure points move if grid is moved
        
        //UnityEngine.Debug.Log("vector" + result);
        
        return result;
    }
}
