using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : PlayerMovement
{
    protected override void Movement() 
    {
        // Movement controls
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }
}
