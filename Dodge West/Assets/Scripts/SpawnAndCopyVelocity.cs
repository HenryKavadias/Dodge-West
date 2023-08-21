using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAndCopyVelocity : Spawner
{
    protected override void CreateObject(GameObject prefab = null)
    {
        var rb = GetComponent<Rigidbody>();

        GameObject mainObject = null;

        Instantiate(prefab, transform.position,
            Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y,
            transform.eulerAngles.z));

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
