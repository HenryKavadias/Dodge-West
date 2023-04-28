using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
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

    protected virtual void Movement() 
    { }

    protected virtual void ApplyGravity()
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

    protected virtual void Jump()
    {
        // Jump controls
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
