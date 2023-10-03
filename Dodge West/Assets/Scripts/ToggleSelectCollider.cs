using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //// Update is called once per frame
    //void Update()
    //{
    //    Toggle();
    //}
}
