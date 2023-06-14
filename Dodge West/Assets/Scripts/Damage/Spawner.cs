using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabObj = null;

    public void Spawn(GameObject prefab)
    {
        if (prefab)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
        else if (prefabObj)
        {
            Instantiate(prefabObj, transform.position, Quaternion.identity);
        }

    }
}
