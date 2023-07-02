using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerPhys : NetworkBehaviour
{
    private Rigidbody _rb;

    public float PlayerSpeed = 2f;

    public override void Spawned()
    {
        Debug.Log("HasInputAuthority: " + HasInputAuthority);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false)
        {
            return;
        }

        InputSystem.Update();

        PlayerInput();
        SpeedControl();

        MovePlayer();
    }

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
    public Transform orientation;

    void PlayerInput()
    {
        if (GetComponent<PlayerInput>())
        {
            horizontalInput = movementInput.x;
            verticalInput = movementInput.y;
            Debug.Log("Movement: NIS");
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            Debug.Log("Movement: OIS");
        }
        
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        _rb.AddForce(moveDirection.normalized * PlayerSpeed * 10f, ForceMode.Force);

        //Debug.Log("Moving");
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > PlayerSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * PlayerSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }

    // Movement control variables
    private Vector2 movementInput = Vector2.zero;

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

}
