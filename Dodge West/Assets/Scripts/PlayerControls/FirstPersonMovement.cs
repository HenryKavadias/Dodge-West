using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonMovement : MonoBehaviour
{    
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public bool enableSprint = false;

    [Header("Jump")]
    public float groundDrag = 0.5f;
    public float airDrag = 0.2f;

    public float jumpForce = 7f;
    public float airMultiplier = 0.4f;
    public float jumpCooldown = 0.25f;
    bool readyToJump;

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    private float currentHeight;
    public LayerMask groundMask;
    bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Other")]
    public GameObject playerModel;

    public Transform orientation;

    float horizontalIput;
    float verticalIput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private Vector2 movementInput = Vector2.zero;
    private bool jumped = false;
    private bool sprinting = false;
    private bool crouching = false;
    private bool stillCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        if (playerModel)
        {
            currentHeight = playerModel.transform.localScale.y * 2;
        }
        else
        {
            Debug.Log("Error, can't get players scale");
        }

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

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprinting = context.action.triggered;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        // Button Hold system
        //crouching = context.action.triggered;

        // Toggle system
        if (crouching)
        {
            crouching = false;
        }
        else
        {
            crouching = true;
        }
    }

    void Update()
    {
        isGrounded = Physics.Raycast(
            transform.position, Vector3.down,
            currentHeight * 0.5f + 0.2f, groundMask);

        PlayerInput();
        
        SpeedControl();

        StateHandler();

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

        // start crouch
        if (crouching)
        {
            transform.localScale = new Vector3(
                transform.localScale.x, 
                crouchYScale, 
                transform.localScale.z);

            if (!stillCrouching)
            {
                rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
                stillCrouching = true;
            }
        }

        // stop crouch
        if (!crouching && stillCrouching)
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                startYScale,
                transform.localScale.z);
            stillCrouching = false;
        }
    }

    private void StateHandler()
    {
        // Mode - Sprinting
        if (isGrounded && sprinting && enableSprint)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (crouching)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        // Mode - Walking
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalIput + orientation.right * horizontalIput;

        // on slope
        // Note: if crouching, movement slower than 5 on a
        // slope of 30 degrees will result in snale paced movement
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        else if (isGrounded)
        {
            // on ground
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            // in the air
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        // Turn gravity off while on slope
        rb.useGravity = !OnSlope();

    }

    void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        // limiting speed on ground and air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    void Jump()
    {
        exitingSlope = true;
        
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(
            transform.position,
            Vector3.down,
            out slopeHit,
            currentHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0f;
        }
        //else if (crouching && Physics.Raycast(
        //    transform.position,
        //    Vector3.down,
        //    out slopeHit,
        //    currentHeight * 0.25f + 0.3f))
        //{
        //    float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
        //    return angle < maxSlopeAngle && angle != 0f;
        //}

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
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
