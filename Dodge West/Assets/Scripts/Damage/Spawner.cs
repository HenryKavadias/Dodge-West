using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabObj = null;

    public virtual void Spawn(GameObject prefab = null)
    {
        if (prefab)
        {
            Instantiate(prefab, transform.position, 
                Quaternion.Euler(transform.eulerAngles.x, 
                transform.eulerAngles.y, 
                transform.eulerAngles.z));
        }
        else if (prefabObj)
        {
            //Instantiate(prefabObj, transform.position, Quaternion.identity);
            Instantiate(prefab, transform.position,
                Quaternion.Euler(transform.eulerAngles.x,
                transform.eulerAngles.y,
                transform.eulerAngles.z));
        }

    }
}
