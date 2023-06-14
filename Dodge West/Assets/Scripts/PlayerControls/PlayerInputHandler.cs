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
    // Reference for local multiplayer controls
    // (Single player controls are with the
    // Player Input component with Unity Events)
    private PlayerConfiguration playerConfig;

    private FirstPersonMovement movement;
    private CameraControl look;
    private PhysicsPickup pickup;
    private Dash dash;
    private PauseControls pause;

    [SerializeField]
    private MeshRenderer playerMesh;

    // For control map checking
    private PlayerControls controls;

    // Get references to all scripts that have the player controls
    private void Awake()
    {
        movement = GetComponent<FirstPersonMovement>();
        look = GetComponent<CameraControl>();
        pickup = GetComponent<PhysicsPickup>();
        dash = GetComponent<Dash>();
        pause = GetComponent<PauseControls>();

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
        // If single player, enable native controls,
        // Otherwise access the script for player controls
        // TODO?: may need to change it to just the script

        if (id == 0)
        {
            // Native controls (Single player)
            GetComponent<PlayerID>().ChangePlayerNumber(id);
            gameObject.GetComponent<PlayerInput>().enabled = true;
        }
        else
        {
            // Player configuration controls (Local multiplayer
            GetComponent<PlayerID>().ChangePlayerNumber(id);

            gameObject.GetComponent<PlayerInput>().enabled = false;

            playerConfig = config;
            playerMesh.material = config.playerMaterial;

            // Assign the player controls to the player configuration
            config.Input.onActionTriggered += Input_onActionTriggered;
        }
    }

    // Assigns the controls to the player configuration with script
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

        // OnPause
        if (actionName == controls.Player.Pause.name)
        {
            OnPause(obj);
        }
    }

    // All the control function references for all the player abilities

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

    public void OnPause(CallbackContext context)
    {
        if (pause)
        {
            pause.OnPause(context);
        }
    }
}
