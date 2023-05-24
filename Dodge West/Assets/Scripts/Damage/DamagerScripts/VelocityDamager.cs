using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectState
{
    Idle,
    Held,
    Thrown
}

[RequireComponent(typeof(Rigidbody))]
public class VelocityDamager : Damager
{
    private Rigidbody rb;
    // used to prevent the carrier from damaging
    // themselves with the object they are carrying
    private GameObject currentHolder = null; 

    private ObjectState state { get; set; } = ObjectState.Idle;

    public void Pickup(GameObject holder)
    {
        currentHolder = holder;
        state = ObjectState.Held;
    }

    public void Drop(bool thrown = false)
    {
        currentHolder = null;
        if (thrown)
        {
            state = ObjectState.Thrown;
        }
        else
        {
            state = ObjectState.Idle;
        }
    }

    public bool IsHeld()
    {
        return currentHolder != null;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    [Header("Default")]
    public float minDamageVelocity = 2f;
    [Range(0f, 1f)] public float velocityThreshold = 0.3f;

    [Header("Thrown")]
    public float minThrownDamageVelocity = 10f;
    [Range(0f, 1f)] public float thrownVelocityThreshold = 0.2f;

    [Header("Held")]
    public float minHeldDamageVelocity = 5f;
    [Range(0f, 1f)] public float heldVelocityThreshold = 0.5f;

    [Header("Idle")]
    public float minIdleDamageVelocity = 3f;
    [Range(0f, 1f)] public float idleVelocityThreshold = 0.7f;

    private void CalculateAndApplyDamage(Damageable damageScript, float minDamageVel, float velThreshold)
    {
        float damageFactor = rb.velocity.magnitude / minDamageVel;

        if (damageFactor > velThreshold)
        {
            damageScript.Damage(damage * damageFactor);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!currentHolder && state != ObjectState.Thrown)
        {
            state = ObjectState.Idle;
        }

        if (collision.gameObject != currentHolder)
        {
            Damageable damageable = collision.gameObject.GetComponent<Damageable>();

            // if object is damageable
            if (damageable)
            {
                // Different damage thresholds for different states
                switch (state)
                {
                    case ObjectState.Idle:

                        CalculateAndApplyDamage(damageable, minIdleDamageVelocity, idleVelocityThreshold);

                        break;
                    case ObjectState.Held:
                        
                        CalculateAndApplyDamage(damageable, minHeldDamageVelocity, heldVelocityThreshold);

                        break;
                    case ObjectState.Thrown:

                        CalculateAndApplyDamage(damageable, minThrownDamageVelocity, thrownVelocityThreshold);

                        break;
                    default:
                        
                        CalculateAndApplyDamage(damageable, minDamageVelocity, velocityThreshold);

                        break;
                }
            }
        }

        if (!currentHolder)
        {
            state = ObjectState.Idle;
        }
    }
}
