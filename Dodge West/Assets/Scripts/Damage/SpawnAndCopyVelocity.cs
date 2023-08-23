using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAndCopyVelocity : Spawner
{
    protected override void CreateObject(GameObject prefab = null)
    {
        var rb = GetComponent<Rigidbody>();

        GameObject mainObject = //null;

        Instantiate(prefab, transform.position,
            Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y,
            transform.eulerAngles.z));

        // Copy the velocity and angular velocity on each fragment of the broken object
        if (mainObject != null && rb != null)
        {
            foreach (Transform obj in mainObject.transform)
            {
                var objRb = obj.gameObject.GetComponent<Rigidbody>();
                if (objRb == null)
                {
                    continue;
                }

                objRb.velocity = rb.velocity;
                objRb.angularVelocity = rb.angularVelocity;
            }
        }
    }
}
