/*
    Author: Ian Rodriguez
    Script Description:
        [[[TO DO]]]
    
    TO DO:
        - script description
        - change DrawSphere to DrawIcon
*/
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private float _pointDensity = 1.0f; //no space between blocks

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetNearestPointOnGrid(Vector3 pos)
    {
        pos -= transform.position; //for making sure points move if grid is moved

        //getting number of points across grid
        int xCount = Mathf.RoundToInt(pos.x / _pointDensity);
        int yCount = Mathf.RoundToInt(pos.y / _pointDensity);
        int zCount = Mathf.RoundToInt(pos.z / _pointDensity);

        //adjusting according to "size"
        Vector3 result = new Vector3(
            (float)xCount * _pointDensity,
            (float)yCount * _pointDensity,
            (float)zCount * _pointDensity);

        result += transform.position; //for making sure points move if grid is moved

        return result;
    }
}
