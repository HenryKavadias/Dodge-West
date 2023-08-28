using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Damageable))]
public class SelfVelocityDamager : Damager
{
    private Rigidbody rb;
    private Damageable damageable;

    [SerializeField]
    private bool limitLayers = false;
    [SerializeField]
    private bool useDefaultLayers = true;
    [SerializeField]
    private LayerMask limitedLayer; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        damageable = GetComponent<Damageable>();

        if (useDefaultLayers) 
        {
            limitedLayer = LayerMask.GetMask("Ground", "Default");
        }
    }

    public float minDamageVelocity = 12;
    [Range(0f, 1f)] public float velocityThreshold = 0.3f;

    private void OnCollisionEnter(Collision collision)
    {
        float damageFactor = rb.velocity.magnitude / minDamageVelocity;

        if (limitLayers)
        {
            //Debug.Log("Limited");

            if ((limitedLayer & 1 << collision.gameObject.layer) == 
                1 << collision.gameObject.layer)
            {
                //Debug.Log("Triggered");
                if (damageFactor > velocityThreshold)
                {
                    damageable.Damage(damage * damageFactor);
                }
            }
        }
        else
        {
            if (damageFactor > velocityThreshold)
            {
                damageable.Damage(damage * damageFactor);
            }
        }
    }
}
