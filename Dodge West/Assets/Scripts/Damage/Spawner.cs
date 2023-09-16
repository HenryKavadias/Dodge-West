using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> prefabObjs = new List<GameObject>();

    public bool assignSounds = false;

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
        if (assignSounds)
        {
            // only assign to first object
            bool firstAssigned = false;

            foreach (GameObject obj in prefabObjs)
            {
                GameObject spawned = Instantiate(obj, transform.position,
                    Quaternion.Euler(transform.eulerAngles.x,
                    transform.eulerAngles.y,
                    transform.eulerAngles.z));

                if (!firstAssigned)
                {
                    if (spawned.GetComponent<AudioController>() != null)
                    {
                        spawned.GetComponent<AudioController>().AssignStartSounds(
                        gameObject.GetComponent<AudioController>().triggerSoundNames);
                    }
                    else
                    {
                        spawned.AddComponent<AudioController>();

                        spawned.GetComponent<AudioController>().AssignStartSounds(
                        gameObject.GetComponent<AudioController>().triggerSoundNames);
                    }

                    firstAssigned = true;
                }
            }
        }
        else
        {
            foreach (GameObject obj in prefabObjs)
            {
                Instantiate(obj, transform.position,
                    Quaternion.Euler(transform.eulerAngles.x,
                    transform.eulerAngles.y,
                    transform.eulerAngles.z));
            }
        }
    }

    protected virtual void CreateObject(GameObject prefab = null)
    {   
        GameObject spawned = Instantiate(prefab, transform.position, 
            Quaternion.Euler(transform.eulerAngles.x, 
            transform.eulerAngles.y, 
            transform.eulerAngles.z));

        if (assignSounds)
        {
            if (spawned.GetComponent<AudioController>() != null)
            {
                spawned.GetComponent<AudioController>().AssignStartSounds(
                gameObject.GetComponent<AudioController>().triggerSoundNames);
            }
            else
            {
                spawned.AddComponent<AudioController>();

                spawned.GetComponent<AudioController>().AssignStartSounds(
                gameObject.GetComponent<AudioController>().triggerSoundNames);
            }
        }
    }
}
