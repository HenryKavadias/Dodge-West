using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAndCopyVelocity : MonoBehaviour
{
    public GameObject prefabObj = null;

    public virtual void Spawn(GameObject prefab = null)
    {
        var rb = GetComponent<Rigidbody>();

        GameObject mainObject = null;

        if (prefab)
        {
            mainObject = Instantiate(prefab, transform.position,
                Quaternion.Euler(transform.eulerAngles.x,
                transform.eulerAngles.y,
                transform.eulerAngles.z));
        }
        else if (prefabObj)
        {
            mainObject = Instantiate(prefab, transform.position,
                Quaternion.Euler(transform.eulerAngles.x,
                transform.eulerAngles.y,
                transform.eulerAngles.z));
        }

        // Copy the velocity and angular velocity on each fragment of the broken object
        if (mainObject != null)
        {
            foreach (Transform obj in mainObject.transform)
            {
                obj.gameObject.GetComponent<Rigidbody>().velocity = rb.velocity;
                obj.gameObject.GetComponent<Rigidbody>().angularVelocity = rb.angularVelocity;
            }
        }
    }
}
