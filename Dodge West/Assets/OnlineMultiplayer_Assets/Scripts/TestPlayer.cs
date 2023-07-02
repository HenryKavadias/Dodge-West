using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayer : NetworkBehaviour
{
    private CharacterController _controller;

    public float PlayerSpeed = 2f;

    private Vector3 velocity;

    public override void Spawned()
    {
        Debug.Log("HasInputAuthority: " + HasInputAuthority);
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false)
        {
            return;
        }

        Vector3 move;

        if (GetComponent<PlayerInput>())
        {
            //InputSystem.Update();

            move = new Vector3(movementInput.x, 0, movementInput.y) * Runner.DeltaTime * PlayerSpeed;

            Debug.Log("Using new input system");
        }
        else
        {
            move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;
        }

        _controller.Move(move + velocity * Runner.DeltaTime);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
    }

    // Movement control variables
    private Vector2 movementInput = Vector2.zero;

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

}
