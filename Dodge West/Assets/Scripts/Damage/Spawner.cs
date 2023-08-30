using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> prefabObjs = new List<GameObject>();

    public void Spawn(GameObject prefab = null)
    {
        if (prefabObjs.Count > 0)
        {
            CreateObjects();
        }
        else if (prefab)
        {
            CreateObject(prefab);
        }
    }

    protected virtual void CreateObjects()
    {
        foreach(GameObject obj in prefabObjs)
        {
            Instantiate(obj, transform.position,
                Quaternion.Euler(transform.eulerAngles.x,
                transform.eulerAngles.y,
                transform.eulerAngles.z));
        }
    }

    protected virtual void CreateObject(GameObject prefab = null)
    {   
        Instantiate(prefab, transform.position, 
            Quaternion.Euler(transform.eulerAngles.x, 
            transform.eulerAngles.y, 
            transform.eulerAngles.z));
    }
}
