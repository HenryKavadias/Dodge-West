using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destroyer : MonoBehaviour
{
    public bool destroyNow = true;
    public float destroyDelay = 1.0f;
    // Destroy object the script is attached to
    public void DestroyThis()
    {
        if (destroyNow)
        {
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject, destroyDelay);
    }
}
