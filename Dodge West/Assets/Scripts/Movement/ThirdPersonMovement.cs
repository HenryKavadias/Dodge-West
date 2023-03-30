using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : PlayerMovement
{
    public float turnSmoothTime = 0.1f;
    public Transform cam;

    float turnSmoothVelocity;

    protected override void Movement()
    {
        // Movement controls
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Cam.eulerAngles.y makes the character follow the direction the camera is faceing. 
            // If you only want this when the player is "shooting" in that direction, make a condition for it
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}
