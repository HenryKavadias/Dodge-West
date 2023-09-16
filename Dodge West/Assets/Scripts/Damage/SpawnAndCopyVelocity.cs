using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAndCopyVelocity : Spawner
{
    [SerializeField]
    private bool disableBeforeSpawn = false;

    protected override void CreateObjects()
    {
        var rb = GetComponent<Rigidbody>();

        if (disableBeforeSpawn)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        // only assign to first object
        bool firstAssigned = false;

        foreach (GameObject obj in prefabObjs)
        {
            GameObject mainObject =

            Instantiate(obj, transform.position,
                Quaternion.Euler(transform.eulerAngles.x,
                transform.eulerAngles.y,
                transform.eulerAngles.z));

            if (assignSounds)
            {
                if (!firstAssigned)
                {
                    if (mainObject.GetComponent<AudioController>() != null)
                    {
                        mainObject.GetComponent<AudioController>().AssignStartSounds(
                        gameObject.GetComponent<AudioController>().triggerSoundNames);
                    }
                    else
                    {
                        mainObject.AddComponent<AudioController>();

                        mainObject.GetComponent<AudioController>().AssignStartSounds(
                        gameObject.GetComponent<AudioController>().triggerSoundNames);
                    }

                    firstAssigned = true;
                }
            }

            // Copy the velocity and angular velocity on each fragment of the broken object
            if (mainObject != null && rb != null)
            {
                foreach (Transform child in mainObject.transform)
                {
                    var objRb = child.gameObject.GetComponent<Rigidbody>();
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

    protected override void CreateObject(GameObject prefab = null)
    {
        var rb = GetComponent<Rigidbody>();

        if (disableBeforeSpawn)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        GameObject mainObject = //null;

        Instantiate(prefab, transform.position,
            Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y,
            transform.eulerAngles.z));

        if (assignSounds)
        {
            if (mainObject.GetComponent<AudioController>() != null)
            {
                mainObject.GetComponent<AudioController>().AssignStartSounds(
                gameObject.GetComponent<AudioController>().triggerSoundNames);
            }
            else
            {
                mainObject.AddComponent<AudioController>();

                mainObject.GetComponent<AudioController>().AssignStartSounds(
                gameObject.GetComponent<AudioController>().triggerSoundNames);
            }
        }

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
