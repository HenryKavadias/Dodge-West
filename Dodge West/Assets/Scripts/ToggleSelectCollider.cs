using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Toggles the enabled state of the assigned collider.
// Used to turn off the select collider (trigger volume) for an object when it's pickedup

// (This is used to prevent the players "preloaded object" from intially inhibiting their movement)
public class ToggleSelectCollider : MonoBehaviour
{
    public Collider selectCollider = null;

    public void Toggle()
    {
        if (selectCollider != null && GetComponent<VelocityDamager>().IsHeld())
        {
            selectCollider.enabled = false;
        }
        else
        {
            selectCollider.enabled = true;
        }
    }
}
