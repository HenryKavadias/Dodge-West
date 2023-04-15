using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class Progressive : MonoBehaviour
{
    [SerializeField] private float _initial;    // initial value
    private float _current;                     // current value

    public float Current
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
            OnChange?.Invoke();
        }
    }

    // Set inital (also is the maximum amount)
    public float Initial => _initial;

    // Return the ratio between current and max (initial)
    public float Ratio => _current / _initial;

    // Used to trigger changes for the visual display (e.g. the health bar,
    // which changes in accordance to the ratio of its assigned progressive object)
    public Action OnChange;

    private void Awake()
    {
        _current = _initial;
    }

    // Decrease current value, not below zero
    public void Sub(float amount)
    {
        Current -= amount;

        OnChange?.Invoke(); // for UI changes

        if (Current < 0)
        {
            Current = 0;
        }
    }

    // Increase current value, not above max (initial)
    public void Add(float amount)
    {
        Current += amount;

        if (Current > Initial)
        {
            Current = Initial;
        }
    }
}
