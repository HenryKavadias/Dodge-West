using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabObj = null;
    public bool defaultRotation = false;
    public bool modifyRotationX = false;

    public float modAmount = 90f;

    public void Spawn(GameObject prefab = null)
    {
        if (prefab)
        {
            CreateObject(prefab);
        }
        else if (prefabObj)
        {
            CreateObject(prefabObj);
        }
    }

    protected virtual void CreateObject(GameObject prefab = null)
    {
        if (defaultRotation)
        {
            Instantiate(prefab, transform.position,
            Quaternion.identity);
            return;
        }
        else if (modifyRotationX)
        {
            Instantiate(prefab, transform.position,
            Quaternion.Euler(transform.eulerAngles.x + modAmount,
            transform.eulerAngles.y,
            transform.eulerAngles.z));
        }
        
        Instantiate(prefab, transform.position, 
            Quaternion.Euler(transform.eulerAngles.x, 
            transform.eulerAngles.y, 
            transform.eulerAngles.z));
    }
}
