using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPickup : NetworkBehaviour
{
    [Networked]
    private bool held { get; set; }

    public override void Spawned()
    {
        held = false;
    }

    public bool IsHeld()
    {
        return held;
    }

    public void Drop()
    {
        held = false;
    }

    public void Pickup()
    {
        held = true;
    }
}
