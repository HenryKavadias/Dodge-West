using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pickupable object states. Used to control the
// sensitivity to damage that the object has
public enum ObjectState
{
    Idle,
    Held,
    Thrown
}

[RequireComponent(typeof(Rigidbody))]
public class VelocityDamager : Damager
{
    public bool loadable = true; // yet to be used
    
    private Rigidbody rb;   // Objects rigidbody

    // used to prevent the carrier from damaging
    // themselves with the object they are carrying
    private GameObject currentHolder = null; 

    // Current object sate
    private ObjectState state { get; set; } = ObjectState.Idle;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        state = ObjectState.Idle;

        // Loadable objects are highlighted in green, ones that aren't are higlighted in red
        if (gameObject.GetComponent<Outline>())
        {
            if (loadable)
            {
                gameObject.GetComponent<Outline>().OutlineColor = Color.green;
            }
            else
            {
                gameObject.GetComponent<Outline>().OutlineColor = Color.red;
            }
        }
    }

    // Set object to picked up/held state and set holder
    public void Pickup(GameObject holder)
    {
        currentHolder = holder;
        state = ObjectState.Held;
    }

    // Set object to a thrown or idle state depending if the object was "thrown" or "dropped"
    public void Drop(bool thrown = false)
    {
        currentHolder = null;
        // Note: might change this to so dropped objects are in a throw state
        if (thrown)
        {
            state = ObjectState.Thrown;
        }
        else
        {
            state = ObjectState.Idle;
        }
    }

    // Check if object is held
    public bool IsHeld()
    {
        return currentHolder != null;
    }

    // Object damage velocities and thresholds (sensitivities) for each object state
    [Header("Default")]
    public float minDamageVelocity = 20f;
    [Range(0f, 1f)] public float velocityThreshold = 0.3f;

    [Header("Thrown")]
    public float minThrownDamageVelocity = 10f;
    [Range(0f, 1f)] public float thrownVelocityThreshold = 0.2f;

    [Header("Held")]
    public float minHeldDamageVelocity = 15f;
    [Range(0f, 1f)] public float heldVelocityThreshold = 0.5f;

    [Header("Idle")]
    public float minIdleDamageVelocity = 30f;
    [Range(0f, 1f)] public float idleVelocityThreshold = 0.7f;

    // Thrown: when object is thrown by a player. Goes to Idle when it collides with another object (after applying damage)

    // Held: when object is held by a player. Goes to Idle when dropped, goes to Thrown when thrown by player

    // Idle: when object isn't being directly interacted with

    // Calculate and apply the damage to the damageable object (if it passes the threshold)
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
        // if object isn't help or in a thrown state, set to Idle
        if (!currentHolder && state != ObjectState.Thrown)
        {
            state = ObjectState.Idle;
        }

        // Avoids damaging the holder of the object
        if (collision.gameObject != currentHolder)
        {
            Damageable damageable = collision.gameObject.GetComponent<Damageable>();

            // If object is damageable
            if (damageable)
            {
                if (gameObject.tag == "Player")
                {
                    //Debug.Log("Player slap");

                    CalculateAndApplyDamage(damageable, minDamageVelocity, velocityThreshold);

                    return;
                }

                // Calcualte the damage based on the current state
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
