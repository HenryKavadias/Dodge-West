using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Damageable))]
public class SelfVelocityDamager : Damager
{
    private Rigidbody rb;
    private Damageable damageable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        damageable = GetComponent<Damageable>();
    }

    public float minDamageVelocity;
    [Range(0f, 1f)] public float velocityThreshold;

    private void OnCollisionEnter(Collision collision)
    {
        float damageFactor = rb.velocity.magnitude / minDamageVelocity;

        if (damageFactor > velocityThreshold)
        {
            damageable.Damage(damage * damageFactor);
        }
    }
}
