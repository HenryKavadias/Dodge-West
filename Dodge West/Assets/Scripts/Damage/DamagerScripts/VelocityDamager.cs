using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VelocityDamager : Damager
{
    private Rigidbody rb;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public float minDamageVelocity;
    [Range(0f, 1f)] public float velocityThreshold;

    private void OnCollisionEnter(Collision collision)
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
