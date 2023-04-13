using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Warning, if you are going to use this script, DON'T use destroythis script on the
// object until the explosion process is finished.
// (possibly use destory by time, or just deactivate the game object)
public class ExplosionDamager : Damager
{
    // TODO: add a means to apply damage to surrounding damageable objects (maybe make a child script)
    
    public float radius = 3;    // radius of explosion
    public float force = 5;     // force multiplier of explosion

    private void OnEnable()
    {
        Explode();
    }

    private void Explode()
    {
        // List of physics collisions
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        // for each object, apply force. For each damageable object, apply damage.
        for(int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out Rigidbody rigidbody))
            {
                Vector3 direction = (colliders[i].transform.position - transform.position).normalized;
                rigidbody.AddForce(direction * force, ForceMode.Impulse);
            }

            if (colliders[i].TryGetComponent(out Damageable damageable)) 
            {
                Damage(damageable);
            }
        }
    }

    // draws the radius of the explosion in the editor
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
