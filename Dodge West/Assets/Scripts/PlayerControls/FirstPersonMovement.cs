using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonMovement : MonoBehaviour
{    
    [Header("Movement")]
    public float moveSpeed = 6f;

    public float groundDrag = 0.5f;
    public float airDrag = 0.2f;

    public float jumpForce = 7f;
    public float airMultiplier = 0.4f;
    public float jumpCooldown = 0.25f;
    bool readyToJump;

    [Header("Ground Check")]

    // character height = scale.y x 2
    public float playerHeight = 3.6f;

    public LayerMask groundMask;
    bool isGrounded;

    public Transform orientation;

    float horizontalIput;
    float verticalIput;

    Vector3 moveDirection;

    Rigidbody rb;

    private Vector2 movementInput = Vector2.zero;
    private bool jumped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered;
    }

    void Update()
    {
        // Grounded check
        isGrounded = Physics.Raycast(
            transform.position, Vector3.down, 
            playerHeight * 0.5f + 0.2f, groundMask);

        PlayerInput();
        
        SpeedControl();


        // Handle drag
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    // For new "Player Input" system (interacts with the Player Input component)
    void PlayerInput()
    {
        horizontalIput = movementInput.x;
        verticalIput = movementInput.y;

        if (jumped && isGrounded && readyToJump)
        {
            readyToJump = false;

            Jump();
            // allows player to keep jump when the jump button is held down
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalIput + orientation.right * horizontalIput;

        if (isGrounded)
        {
            // on ground
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            // in the air
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    //  Rate in which the agent slows down to a stop when there is no movement input
    protected float decelerationRate = 0.5f;    // Must be between 0 and 1

    // Reduces the velocity of the game object to zero over time
    public void DecelerateVelocity()
    {
        rb.velocity = rb.velocity * decelerationRate * Time.deltaTime;
        rb.angularVelocity = rb.angularVelocity * decelerationRate * Time.deltaTime;
    }
}