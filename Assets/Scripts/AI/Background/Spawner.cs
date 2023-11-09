using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Cameras")]
    /* OVR >> OVRCameraRig 1 >> TrackingSpace >> CenterEyeAnchor */
    public Camera occulusCamera;
    /* Windows >> Camera */
    public Camera windowsCamera;

    // When Called Should Spawn Item In front of player
    public GameObject Spawn(GameObject itemPrefab)
    {
        Vector3 playerPos;
        Vector3 playerDir;
        Quaternion playerRot;


#if !UNITY_STANDALONE_WIN
        playerPos = occulusCamera.transform.position;
        playerDir = occulusCamera.transform.forward;
        playerRot = occulusCamera.transform.rotation;
#else
        playerPos = windowsCamera.transform.position;
        playerDir = windowsCamera.transform.forward;
        playerRot = windowsCamera.transform.rotation;
#endif
        // 1 - 5 - 7 - 10 
        float spawnDist = 5;
        Vector3 spawnPos = playerPos + playerDir * spawnDist;


        return (GameObject)Instantiate(itemPrefab, spawnPos, playerRot);
    }
    public void Despawn(GameObject gameObject)
    {
        Destroy(gameObject);
        Debug.Log("Item Destroyed");
    }
}
