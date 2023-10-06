using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unparents all objects directly parented under this object after a delay.
// If enabled to, object will destroy itself after detaching children
public class Unparent : MonoBehaviour
{
    public bool destroy = true;
    public float detachDestoryDelay = 2f;
    void Start()
    {
        Invoke(nameof(DetachAndDestroy), detachDestoryDelay);
    }

    void DetachAndDestroy()
    {
        transform.DetachChildren();

        if (destroy)
        {
            Destroy(gameObject, detachDestoryDelay);
        }
    }
}
