using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastNetwork : NetworkBehaviour
{
    public RaycastHit CheckRaycast(Ray ray)
    {
        Runner.GetPhysicsScene().Raycast(
            ray.origin, ray.direction, out var hit);
        return hit;
    }

}
