using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    // For local multiplayer controls
    // (Single player controls are with the
    // Player Input component with Unity Events)
    private PlayerConfiguration playerConfig;

    private FirstPersonMovement movement;
    private MouseLook look;
    private PhysicsPickup pickup;
    private Dash dash;

    [SerializeField]
    private MeshRenderer playerMesh;

    // For control map checking
    private PlayerControls controls;

    private void Awake()
    {
        movement = GetComponent<FirstPersonMovement>();
        look = GetComponent<MouseLook>();
        pickup = GetComponent<PhysicsPickup>();
        dash = GetComponent<Dash>();

        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Player ID needs to be set first before this function is called
    public void InitializePlayer(PlayerConfiguration config = null, int id = 0)
    {
        // if single player, enable native controls,
        // Otherwise access the script for player controls
        // TODO?: may need to change it to just the script

        if (id == 0)
        {
            GetComponent<PlayerID>().ChangePlayerNumber(id);
            gameObject.GetComponent<PlayerInput>().enabled = true;
        }
        else
        {
            GetComponent<PlayerID>().ChangePlayerNumber(id);

            gameObject.GetComponent<PlayerInput>().enabled = false;

            playerConfig = config;
            playerMesh.material = config.playerMaterial;
            config.Input.onActionTriggered += Input_onActionTriggered;
        }
    }

    private void Input_onActionTriggered(CallbackContext obj)
    {
        string actionName = obj.action.name;

        // Move
        if (actionName == controls.Player.Movement.name)
        {
            OnMove(obj);
        }

        // Jump
        if (actionName == controls.Player.Jump.name)
        {
            OnJump(obj);
        }

        // Look
        if (actionName == controls.Player.Look.name)
        {
            OnLook(obj);
        }

        // Pickup
        if (actionName == controls.Player.Pickup.name)
        {
            OnPickup(obj);
        }

        // OnThrow
        if (actionName == controls.Player.Throw.name)
        {
            OnThrow(obj);
        }

        // OnSprint
        if (actionName == controls.Player.Sprint.name)
        {
            OnSprint(obj);
        }

        // OnCrouch
        if (actionName == controls.Player.Crouch.name)
        {
            OnCrouch(obj);
        }

        // OnDash
        if (actionName == controls.Player.Dash.name)
        {
            OnDash(obj);
        }

        // OnLoadItem
        if (actionName == controls.Player.LoadItem.name)
        {
            OnLoadItem(obj);
        }
    }

    private void OnMove(CallbackContext context)
    {
        if (movement)
        {
            movement.OnMove(context);
        }
    }

    private void OnJump(CallbackContext context)
    {
        if (movement)
        {
            movement.OnJump(context);
        }
    }

    public void OnLook(CallbackContext context)
    {
        if (look) 
        { 
            look.OnLook(context); 
        }
    }

    public void OnPickup(CallbackContext context)
    {
        if (pickup)
        {
            pickup.OnPickup(context);
        }
    }

    public void OnThrow(CallbackContext context)
    {
        if (pickup)
        {
            pickup.OnThrow(context);
        }
    }

    public void OnSprint(CallbackContext context)
    {
        if (movement)
        {
            movement.OnSprint(context);
        }
    }

    public void OnCrouch(CallbackContext context)
    {
        if (movement)
        {
            movement.OnCrouch(context);
        }
    }

    public void OnDash(CallbackContext context)
    {
        if (dash)
        {
            dash.OnDash(context);
        }
    }

    public void OnLoadItem(CallbackContext context)
    {
        if (pickup)
        {
            pickup.OnLoadItem(context);
        }
    }
}
