using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // When Called Should Spawn Item
    public GameObject Spawn(GameObject itemPrefab)
    {
        return (GameObject)Instantiate(itemPrefab, transform.position, Quaternion.identity);
    }
    public void Despawn(GameObject gameObject)
    {
        Destroy(gameObject);
        Debug.Log("Item Destroyed");
    }
}
