using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using DG.Tweening;

public class MouseLook : MonoBehaviour
{
    [Range(1f, 100f)]
    public float mouseSensitivity = 20f;

    public Transform camPos;

    public Transform orientation;   // Stores the direction the character is facing

    private float xRotation = 0f;
    private float yRotation = 0f;

    private Vector2 mouseInput = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
        //mouseInput = Mouse.current.delta.ReadValue();
    }

    // Update is called once per frame
    void Update()
    {
        LookWithMouse();
    }

    void LookWithMouse()
    {
        float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Changes camera rotation
        camPos.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        // Use to rotate player
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

        // Changes the direction the player character is facing
        //cam.rotation = Quaternion.Euler(xRotation, 0f, 0f);
        //transform.Rotate(Vector3.up * mouseX);
    }

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
