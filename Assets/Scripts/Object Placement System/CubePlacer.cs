/*
    Author: Ian Rodriguez
    Script Description:
        [[[TO DO]]]
    
    TO DO:
        - include illegal placement check against currently existing occupied points (bitmaps)
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
                //PlaceCube(hitInfo.point);
            }
        }
    }

    private void PlaceCube(ObjectData objData)
    {
        //checking if pos is illegal position
        if(objData.success == ParseSuccess.False)
        {
            //[[[AI ERROR MESSAGE]]]
            return;
        }
        if(objData.position.x > 49 || objData.position.x < 1 
            || objData.position.y > 49 || objData.position.y < 1
            || objData.position.z > 49 || objData.position.z < 1)
        {
            //[[[AI ERROR MESSAGE]]]
            return;
        }
        //[[[INCLUDE IF-CHECK FOR TRYING TO PLACE AT OCCUPIED SPACE]]]

        //placing
        Vector3 adjustedPos = _grid.GetNearestPointOnGrid(objData.position);
        GameObject block = GameObject.Instantiate(_block, objData.position, Quaternion.identity);
        block.transform.position = adjustedPos;
        FindAndCreateParent(block);
    }

    private void FindAndCreateParent(GameObject obj)
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
