using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public float turnSmoothTime = 0.1f;
    public Transform cam;

    float turnSmoothVelocity;

    public float speed = 6f;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    protected CharacterController controller;

    protected Vector3 velocity;
    protected bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        LockCursor();

        CharacterController tempCon = GetComponent<CharacterController>();

        if (tempCon != null)
        {
            controller = tempCon;
        }
        else
        {
            Debug.Log("Character Controller component is missing from object");
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();
        Movement();
        Jump();
    }

    void ApplyGravity()
    {
        // Grounded check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        // Jump controls
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }


    void Movement()
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
