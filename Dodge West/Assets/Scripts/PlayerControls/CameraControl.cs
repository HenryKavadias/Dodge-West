using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using DG.Tweening;

// Manages the camera controls for the player
public class CameraControl : MonoBehaviour
{
    // Sensitivity of camera controls (x10 for Gamepad controls, done in input manager)
    [Range(1f, 100f)]
    public float cameraSensitivity = 20f;

    public Transform camPos;        // Camera position reference
    public Transform orientation;   // Stores the direction the character is facing
    public GameObject model;        //  Reference to character model

    // Current rotation of camera
    private float xRotation = 0f;
    private float yRotation = 0f;

    // Current input of camera controls
    private Vector2 lookInput = Vector2.zero;

    // Turn off the cursor
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Gets input values from camera control inputs
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        //mouseInput = Mouse.current.delta.ReadValue();
    }

    void Update()
    {
        LookControls();
    }

    // Manages the direction of the player camera based on camera control inputs
    void LookControls()
    {
        float lookX = lookInput.x * cameraSensitivity * Time.deltaTime;
        float lookY = lookInput.y * cameraSensitivity * Time.deltaTime;

        yRotation += lookX;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Changes camera rotation
        camPos.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        // Use to rotate player
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

        // Apply player rotation to model
        model.transform.rotation = orientation.rotation;
    }

    // Modifies the field of view for the camera
    public void DoFov(float endValue)
    {
        Camera cam = gameObject.GetComponent<CameraManager>().currentCam.GetComponent<Camera>();
        if (cam) 
        {
            // Instant way
            //cam.fieldOfView = endValue;

            cam.DOFieldOfView(endValue, 0.40f);
        }
    }
}
