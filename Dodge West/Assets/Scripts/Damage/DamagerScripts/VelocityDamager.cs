using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VelocityDamager : Damager
{
    private Rigidbody rb;
    // used to prevent the carrier from damaging
    // themselves with the object they are carrying
    private GameObject currentHolder = null; 

    public void Pickup(GameObject holder)
    {
        currentHolder = holder;
    }

    public void Drop()
    {
        currentHolder = null;
    }

    public bool IsHeld()
    {
        return currentHolder != null;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public float minDamageVelocity;
    [Range(0f, 1f)] public float velocityThreshold;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != currentHolder)
        {
            Damageable damageable = collision.gameObject.GetComponent<Damageable>();

            // if object is damageable
            if (damageable)
            {
                float damageFactor = rb.velocity.magnitude / minDamageVelocity;

                if (damageFactor > velocityThreshold)
                {
                    damageable.Damage(damage * damageFactor);
                }
            }
        }
    }
}
