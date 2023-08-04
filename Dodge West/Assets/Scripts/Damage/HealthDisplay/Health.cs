using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Interfaces currently not in use
public interface IDamageable
{
    void Damage(float amount);
}

public interface IHealable
{
    void Heal(float amount);
}

public class Health : Progressive //, IDamageable, IHealable
{
    public UnityEvent onDamaged;
    
    // Decrease current value, not below zero
    public override void Sub(float amount)
    {
        float preChanged = Current;
        
        Current -= amount;

        // Triggers the damage indicator
        // if the health value has changed
        if (preChanged != Current)
        {
            onDamaged?.Invoke();
        }

        if (Current < 0)
        {
            Current = 0;
        }
    }
}
