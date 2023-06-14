using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destroyer : MonoBehaviour
{
    // Destroy object the script is attached to
    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
