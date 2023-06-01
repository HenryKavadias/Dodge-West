using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script will need some refining before implementation
public class ObjectSticker : MonoBehaviour
{
    private bool targetHit;

    private void OnCollisionEnter(Collision collision)
    {
        // make sure only to stick to the first target it hits
        if (targetHit)
        {
            return;
        }
        else
        {
            StickObject(collision);
        }
    }

    void StickObject(Collision collision)
    {
        targetHit = true;

        // makes object stick to what it collides with
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        // makes object move with target
        transform.SetParent(collision.transform);
    }

    void UnStickObject(Collision collision)
    {
        targetHit = false;

        gameObject.GetComponent<Rigidbody>().isKinematic = false;

        transform.parent = null;

        //transform.DetachChildren();
    }
}
