using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destroyer : MonoBehaviour
{
    public bool destoryOnStart = false;
    public bool destroyInstantly = true;
    public float destroyDelay = 1.0f;

    private void Start()
    {
        if (destoryOnStart) 
        {
            DestroyThis();
        }
    }

    // Destroy object the script is attached to
    public void DestroyThis()
    {
        if (destroyInstantly)
        {
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject, destroyDelay);
    }
}
