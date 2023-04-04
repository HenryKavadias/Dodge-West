using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Range(1f, 1000f)]
    public float mouseSensitivity = 300f;

    public Transform cam;

    public Transform orientation;   // Stores the direction the character is facing

    private float xRotation = 0f;
    private float yRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        LookWithMouse();
    }

    void LookWithMouse()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Changes camera rotation
        cam.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Use to rotate player
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

        // Changes the direction the player character is facing
        //cam.rotation = Quaternion.Euler(xRotation, 0f, 0f);
        //transform.Rotate(Vector3.up * mouseX);
    }
}
