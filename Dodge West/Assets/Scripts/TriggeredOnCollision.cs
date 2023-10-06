using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Triggers a function (unity event) when this object collides with something else
public class TriggeredOnCollision : MonoBehaviour
{
    public float minVelocity = 1.0f;
    [Range(0.0f, 1.0f)] public float velocityThreshold = 0.1f;
    public bool enableLimit = true;

    public UnityEvent onCollision;

    private void OnCollisionEnter(Collision collision)
    {
        if (enableLimit)
        {
            float factor = gameObject.GetComponent<Rigidbody>().velocity.magnitude / minVelocity;

            if (factor > velocityThreshold)
            {
                onCollision?.Invoke();
            }
        }
        else
        {
            onCollision?.Invoke();
        }
    }
}
