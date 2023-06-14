using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// Warning, if this object has and post death processes, DON'T use destroythis script
// on the object until those process is finished.
// (possibly use destory by time, or just deactivate the game object)
public class Damageable : MonoBehaviour
{
    [SerializeField] protected Progressive _health;

    // Destroy, spawn, or do something depending on the event
    public UnityEvent OnDie;

    // Damage object
    public virtual void Damage(
        float damage, GameObject attacker = null)
    {
        // prevents overlapping hit ties
        if (_health.Current <= 0)
        {
            return;
        }

        _health.Sub(damage);

        if (_health.Current <= 0)
        {
            //Die();
            StartCoroutine(SlowDie());
        }
    }

    // Heal object
    public virtual void Heal(float heal)
    {
        _health.Add(heal);
    }

    // Trigger OnDie event immediately 
    protected void Die()
    {
        OnDie.Invoke();
    }

    // Trigger OnDie event after next frame
    protected IEnumerator SlowDie()
    {
        yield return null;
        OnDie.Invoke();
    }
}
