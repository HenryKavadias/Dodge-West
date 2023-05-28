using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LifeCounter : MonoBehaviour
{
    [SerializeField] private int _initial;    // initial value
    private int _current;                     // current value

    public UnityEvent OnZeroLives;

    public UnityEvent OnLooseALife;

    public int Current
    {
        get
        {
            return _current;
        }
        set
        {
            _current = value;
            // Used to change the display bar for the value
            // without needing to rely on an update function.
            // (Saves on processing space)
            OnCountChange?.Invoke();
        }
    }

    public void SetLives(int lives)
    {
        _current = lives;
    }

    // Set inital (also is the maximum amount)
    public int Initial => _initial;

    // Return the ratio between current and max (initial)
    public float Ratio => _current / _initial;

    // Used to trigger changes for the visual display (LifeDisplay)
    public Action OnCountChange;

    private void Awake()
    {
        _current = _initial;
    }

    // Decrease current value, not below zero
    public void Sub(int amount)
    {
        Current -= amount;

        if (Current < 0)
        {
            Current = 0;
        }

        if (Current <= 0)
        {
            StartCoroutine(SlowTriggerDead());
        }
        else
        {
            StartCoroutine(SlowTriggerLoose());
        }
    }

    // Increase current value, not above max (initial)
    public void Add(int amount)
    {
        Current += amount;

        if (Current > Initial)
        {
            Current = Initial;
        }
    }

    private void TriggerDead()
    {
        OnZeroLives.Invoke();
    }

    private IEnumerator SlowTriggerDead()
    {
        yield return null;
        OnZeroLives.Invoke();
    }

    private IEnumerator SlowTriggerLoose()
    {
        yield return null;
        OnLooseALife.Invoke();
    }
}
