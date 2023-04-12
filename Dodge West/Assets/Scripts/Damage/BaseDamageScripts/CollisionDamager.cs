using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamager : Damager
{
    private void OnCollisionEnter(Collision collision)
    {
        Damageable damageable = collision.collider.GetComponent<Damageable>();

        if (damageable)
        {
            damageable.Damage(damage);
        }
    }
}