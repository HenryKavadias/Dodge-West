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

    // makes the object temporarily invincible on spawn
    [SerializeField] protected bool invincible = true;
    [SerializeField] protected float invincibleTime = 0.2f;

    private void Start()
    {
        if (_health == null)
        {
            _health = gameObject.GetComponent<Progressive>();
        }

        Invoke(nameof(TurnOffInvincible), invincibleTime);
    }

    protected void TurnOffInvincible()
    {
        invincible = false;
    }

    // Damage object
    public virtual void Damage(
        float damage, GameObject attacker = null)
    {
        // prevents overlapping hit ties
        if (_health.Current <= 0 || invincible)
        {
            return;
        }

        _health.Sub(damage);

        //if (_health.Current <= 0)
        //{
        //    //Die();
        //    //StartCoroutine(SlowDie());
        //}
    }

    // Heal object
    public virtual void Heal(float heal)
    {
        _health.Add(heal);
    }

    //// Destroy, spawn, or do something depending on the event
    //public UnityEvent onDie;

    //// Trigger OnDie event immediately 
    //protected void Die()
    //{
    //    onDie.Invoke();
    //}

    //// Trigger OnDie event after next frame
    //protected IEnumerator SlowDie()
    //{
    //    yield return null;
    //    onDie.Invoke();
    //}
}
