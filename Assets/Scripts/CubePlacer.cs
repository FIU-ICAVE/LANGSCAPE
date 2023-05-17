/*
    Author: Ian Rodriguez
    Script Description:
        [[[TO DO]]]
    
    TO DO:
        - update boundary check to include lower bounds
        - script description
        - comment out Update() code once LLM can make placement requests
        - out of bounds request: inform user
        - change name of PlaceCube() so that it is appropriate to be used with other objects
*/
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class CubePlacer : MonoBehaviour
{
    private Grid _grid;
    [SerializeField]
    private GameObject _block;
    private GameObject _objectContainer;

    // Start is called before the first frame update
    void Start()
    {
        _grid = GameObject.FindObjectOfType<Grid>();
        if(_grid == null)
        {
            Debug.LogError("CubePlacer::Start - Grid component is null");
        }
        _objectContainer = GameObject.FindGameObjectWithTag("ObjectContainer");
        if(_objectContainer == null)
        {
            Debug.LogError("CubePlacer::Start - Object Container is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //[[[for display purposes ONLY]]]
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hitInfo))
            {
                PlaceCube(hitInfo.point);
            }
        }
    }

    private void PlaceCube(Vector3 pos)
    {
        //checking if pos is legal position
        if(pos.x > 50
            || pos.y > 50
            || pos.z > 50)
        {
            //[[[INCLUDE ERROR MESSAGE BACK TO USER]]]
            return;
        }

        //placing
        Vector3 adjustedPos = _grid.GetNearestPointOnGrid(pos);
        GameObject block = GameObject.Instantiate(_block, pos, Quaternion.identity);
        block.transform.position = adjustedPos;
        FindParent(block);
    }

    private void FindParent(GameObject obj)
    {
        GameObject parent;
        string parentName = obj.name + "s";

        parent = GameObject.Find(parentName);

        //creating parent if already does not exist
        if (parent == null)
        {
            parent = new GameObject(parentName);
            parent.transform.SetParent(_objectContainer.transform); //putting new parent under "Placed_Objects"
        }

        obj.transform.SetParent(parent.transform);
    }
}
