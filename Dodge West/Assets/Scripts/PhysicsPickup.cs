using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPickup : MonoBehaviour
{
    [SerializeField] private LayerMask pickupMask;
    private Camera playerCamera;
    [SerializeField] private Transform pickupTarget;
    [Space]
    [SerializeField] private float pickupRange;
    [SerializeField] private float objectSpeedModifier = 12f;
    [SerializeField] private float maxObjectSpeed = 20f;
    [SerializeField] private float throwPower = 25f;
    private Rigidbody currentObject;
    
    public void SetCamera(Camera cam)
    {
        playerCamera = cam;
    }

    // Update is called once per frame
    void Update()
    {
        // Pickup object
        if (Input.GetButtonDown("Pickup"))
        {
            // Drop object
            if (currentObject)
            {
                currentObject.useGravity = true;
                currentObject = null;
                return;
            }
            else
            {
                Ray cameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                if (Physics.Raycast(cameraRay, out RaycastHit hitInfo, pickupRange, pickupMask))
                {
                    currentObject = hitInfo.rigidbody;
                    currentObject.useGravity = false;

                    return;
                }
            }
        }

        if (Input.GetButtonDown("Fire1") && currentObject)
        {
            currentObject.useGravity = true;
            currentObject.AddForce(pickupTarget.forward * throwPower, ForceMode.Impulse);
            currentObject = null;
            return;
        }
    }

    void FixedUpdate()
    {
        if (currentObject)
        {
            Vector3 directionToPoint = pickupTarget.position - currentObject.position;
            float distanceToPoint = directionToPoint.magnitude;

            currentObject.velocity = directionToPoint * distanceToPoint * objectSpeedModifier;

            if (currentObject.velocity.magnitude > maxObjectSpeed)
            {
                currentObject.velocity = Vector3.ClampMagnitude(currentObject.velocity, maxObjectSpeed);
            }

            //Mathf.Clamp()
        }
    }
}
