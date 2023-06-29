using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

// Manages basic movement for the player
// (camera controls are done in the camera controls script)

// Note: movement is done with physics manipulation of the
// players rigidbody, NOT manipulation of the players transform
public class FirstPersonMovement : MonoBehaviour
{    
    // Variables for first person movement controls

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public bool enableSprint = false;

    // Dash variables (used by dash script)
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    public float maxYSpeed;
    public float groundDrag = 0.5f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float airMultiplier = 0.4f;
    public float jumpCooldown = 0.25f;
    bool readyToJump;

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public Collider playerCollider;
    public float distanceOfCheck = 0.2f;
    [Range(0.001f, 1f)]
    public float groundCheckBoxSizeMultiplier = 0.8f;
    private float currentHeight;
    private Vector3 groundCheckBoxSize;
    protected bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Other")]
    public GameObject playerModel;
    public Transform orientation;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    protected Rigidbody rb;

    // Manages the movement states of the player
    // (Used to manage the different physics forces applied to the player)
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        dashing,
        air
    }

    // Tracks if the player is dashing
    public bool dashing;

    // Movement control variables
    private Vector2 movementInput = Vector2.zero;
    public Vector2 currentMoveInput
    {
        get { return movementInput; }
    }
    private bool jumped = false;
    private bool sprinting = false;
    protected bool crouching = false;
    private bool stillCrouching = false;

    public void SetRigidbody()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        // Used for crouching
        startYScale = transform.localScale.y;

        // Get player character height and set the ground check size
        if (playerModel)
        {
            currentHeight = playerModel.transform.localScale.y * 2;
            // Note: also used for Sphere Cast
            groundCheckBoxSize = new Vector3(
                playerModel.transform.localScale.x * groundCheckBoxSizeMultiplier
                , 0.05f, 
                playerModel.transform.localScale.z * groundCheckBoxSizeMultiplier);
        }
        else
        {
            Debug.Log("Error, can't get players scale");
        }

        // Disables cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Movement controls functions
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

    public virtual void OnCrouch(InputAction.CallbackContext context)
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

    // Shows the ground box cast check
    private void OnDrawGizmos()
    {
        float maxDistance = currentHeight * 0.5f + distanceOfCheck;
        RaycastHit hit;

        bool isHit = Physics.BoxCast(
            playerCollider.bounds.center,
            new Vector3(0.2f, 0.2f, 0.2f),
            Vector3.down,
            out hit,
            transform.rotation,
            maxDistance,
            groundMask);

        if (isHit)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.down * hit.distance);
            //Gizmos.DrawWireCube(transform.position + (Vector3.down * hit.distance), groundCheckBoxSize);
            Gizmos.DrawWireSphere(transform.position + (Vector3.down * maxDistance), groundCheckBoxSize.x / 2);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * maxDistance);
        //Gizmos.DrawWireCube(transform.position + (Vector3.down * maxDistance), groundCheckBoxSize);
        Gizmos.DrawWireSphere(transform.position + (Vector3.down * maxDistance), groundCheckBoxSize.x / 2);

    }
    protected bool RayCastCheck()
    {
        return Physics.Raycast(
            transform.position, Vector3.down,
            currentHeight * 0.5f + distanceOfCheck, 
            groundMask);
    }

    protected bool BoxCastCheck()
    {
        return Physics.BoxCast(
            playerCollider.bounds.center,
            groundCheckBoxSize,
            Vector3.down,
            transform.rotation,
            currentHeight * 0.5f + distanceOfCheck,
            groundMask);
    }

    // This is currently used for ground checking
    protected bool SphereCastCheck()
    {
        return Physics.SphereCast(
            playerCollider.bounds.center,
            groundCheckBoxSize.x / 2,
            Vector3.down, out RaycastHit hit,
            currentHeight * 0.5f + distanceOfCheck,
            groundMask);
    }

    // Used for player inputs
    protected virtual void Update()
    {
        // Checks if the player is touching the ground
        isGrounded = SphereCastCheck();

        PlayerInput();
        SpeedControl();
        StateHandler();

        // Handles drag
        if (state == MovementState.walking || 
            state == MovementState.sprinting || 
            state == MovementState.crouching)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    // Used for player movement (based on physics applied to the rigidbody)
    protected virtual void FixedUpdate()
    {
        MovePlayer();
    }

    // Handles the inputs of the player (interacts with the Player Input component)
    protected void PlayerInput()
    {
        // Handles 2D player movement
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;

        // Handles jump
        if (jumped && isGrounded && readyToJump)
        {   
            readyToJump = false;

            Jump();
            // allows player to keep jump when the jump button is held down
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Start crouching
        if (crouching)
        {
            // Modify character model (NOTE: may get swapped with an animation)
            transform.localScale = new Vector3(
                transform.localScale.x, 
                crouchYScale, 
                transform.localScale.z);

            // Forces the player character to the ground
            if (!stillCrouching)
            {
                rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
                stillCrouching = true;
            }
        }

        // Stop crouching 
        if (!crouching && stillCrouching)
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                startYScale,
                transform.localScale.z);
            stillCrouching = false;
        }
    }

    // Physics movement variables
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    // Handles the movement state of the player
    protected void StateHandler()
    {
        // Mode - Dashing
        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }
        // Mode - Sprinting
        else if (isGrounded && sprinting && enableSprint)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        // Mode crouching
        else if (isGrounded && crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        // Mode - Walking
        else if (isGrounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;

            if(desiredMoveSpeed < sprintSpeed)
            {
                desiredMoveSpeed = walkSpeed;
            }
            else { desiredMoveSpeed = sprintSpeed; }
        }

        // Tracks if desired player movement speed speed
        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (lastState == MovementState.dashing) { keepMomentum = true; }

        // Controls the smooth change of the movement speed to desired value
        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private float speedChangeFactor;

    // Smoothly lerp movementSpeed to desired value
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    // Controls the physics of how the player is moving based on the movement state
    protected void MovePlayer()
    {
        if (state == MovementState.dashing) { return; }
        
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On slope
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

    // Controls the speed of the player under certin circumstances
    protected void SpeedControl()
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

        // limit y velocity
        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
        }
    }

    // Controls the physics behind the jump ability
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

    // Controls how the player moves up and down slopes
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

        return false;
    }

    // Get the direct the player is moving on a slope
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
