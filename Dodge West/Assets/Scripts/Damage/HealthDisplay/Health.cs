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
    //[SerializeField] private UnityEvent OnDie;
    
    //public void Damage(float amount)
    //{
    //    Current -= amount;

    //    OnChange?.Invoke();

    //    if (Current <= 0) 
    //    {
    //        OnDie?.Invoke();
    //    }
    //}

    //public void Heal(float amount)
    //{
    //    Current += amount;

    //    if (Current > Initial)
    //    {
    //        Current = Initial;
    //    }
    //}
}
