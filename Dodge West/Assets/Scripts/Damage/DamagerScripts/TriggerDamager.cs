using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TriggerDamager : Damager
{
    private void OnTriggerEnter(Collider other)
    {
        Damageable damageable = other.transform.root.GetComponent<Damageable>();

        if (damageable)
        {
            damageable.Damage(damage);
        }
    }
}
