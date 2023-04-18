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
    [SerializeField] private float objectTrackingSpeedModifier = 12f;
    [SerializeField] private float maxObjectSpeed = 20f;
    [SerializeField] private float throwPower = 25f;
    private Rigidbody currentObject;
    
    public void SetCamera(Camera cam)
    {
        playerCamera = cam;
    }
    
    void Update()
    {
        // When picking up or dropping an object you must modifiy 3 of it attributes:
        // - It's Velocity damager script, tell it if it's pickup or dropped
        // - Use gravity if dropped, don't use if picked up
        // - the reference of "currentObject", null if dropped, set reference to picked up object
        if (Input.GetButtonDown("Pickup"))
        {
            // Drop object
            if (currentObject)
            {
                currentObject.GetComponent<VelocityDamager>().Drop();
                currentObject.useGravity = true;
                currentObject = null;
                return;
            }
            else
            {
                // Pickup object
                Ray cameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                if (Physics.Raycast(cameraRay, out RaycastHit hitInfo, pickupRange, pickupMask))
                {
                    currentObject = hitInfo.rigidbody;
                    currentObject.GetComponent<VelocityDamager>().Pickup(gameObject);
                    currentObject.useGravity = false;

                    return;
                }
            }
        }

        // Throw object
        if (Input.GetButtonDown("Fire1") && currentObject)
        {
            currentObject.GetComponent<VelocityDamager>().Drop();
            currentObject.useGravity = true;
            currentObject.AddForce(pickupTarget.forward * throwPower, ForceMode.Impulse);
            currentObject = null;
            return;
        }
    }

    void FixedUpdate()
    {
        // Makes current object travel to the pick up point of the player
        if (currentObject)
        {
            Vector3 directionToPoint = pickupTarget.position - currentObject.position;
            float distanceToPoint = directionToPoint.magnitude;

            currentObject.velocity = directionToPoint * distanceToPoint * objectTrackingSpeedModifier;

            if (currentObject.velocity.magnitude > maxObjectSpeed)
            {
                currentObject.velocity = Vector3.ClampMagnitude(currentObject.velocity, maxObjectSpeed);
            }

            //Mathf.Clamp()
        }
    }
}
