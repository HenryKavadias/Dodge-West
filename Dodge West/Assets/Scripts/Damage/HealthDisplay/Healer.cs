using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Healer : MonoBehaviour
{
    public float heal;  // Heal amount  

    // Apply heal to damageable object
    public void Heal(Damageable damageable) => damageable.Heal(heal);

    //protected void ApplyHeal(IHealable Healable) => Healable.Heal(heal);

    // Its damage counterpart is under Damager object
}
