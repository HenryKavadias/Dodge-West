using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unparent : MonoBehaviour
{
    public bool destroy = true;
    void Start()
    {
        transform.DetachChildren();

        if (destroy)
        {
            Destroy(gameObject, 2f);
        }
    }
}
