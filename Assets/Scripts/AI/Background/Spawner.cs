using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // When Called Should Spawn Item
    public GameObject Spawn(GameObject itemPrefab)
    {
<<<<<<< Updated upstream
        return (GameObject)Instantiate(itemPrefab, transform.position, Quaternion.identity);
=======
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
>>>>>>> Stashed changes
    }
    public void Despawn(GameObject gameObject)
    {
        Destroy(gameObject);
        Debug.Log("Item Destroyed");
    }
}
