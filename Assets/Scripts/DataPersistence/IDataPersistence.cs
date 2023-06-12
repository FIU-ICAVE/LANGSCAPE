using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(GameObject data);

    void SaveData(ref GameObject data);
}
