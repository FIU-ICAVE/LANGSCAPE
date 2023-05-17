/*
    Author: Ian Rodriguez
    Script Description:
        [[[TO DO]]]
    
    TO DO:
        - make distance between points nearly side-by-side
        - script description
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
    private float _size = 1.0f;
    [SerializeField]
    private bool _gridGizmoDebug = true;

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
        int xCount = Mathf.RoundToInt(pos.x / _size);
        int yCount = Mathf.RoundToInt(pos.y / _size);
        int zCount = Mathf.RoundToInt(pos.z / _size);

        //adjusting according to "size"
        Vector3 result = new Vector3(
            (float)xCount * _size,
            (float)yCount * _size,
            (float)zCount * _size);

        result += transform.position; //for making sure points move if grid is moved

        return result;
    }

    private void OnDrawGizmos()
    {
        if (_gridGizmoDebug)
        {
            Gizmos.color = Color.red;
            for (float y = 0; y < 10; y += _size)
            {
                for (float x = 0; x < 10; x += _size)
                {
                    for (float z = 0; z < 10; z += _size)
                    {
                        Vector3 point = GetNearestPointOnGrid(new Vector3(x, y, z));
                        Gizmos.DrawSphere(point, 0.1f);
                    }
                }
            }
        } 
    }
}
