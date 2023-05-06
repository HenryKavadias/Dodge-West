using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private bool pickedup = false;
    private bool thrown = false;

    public void OnPickup(InputAction.CallbackContext context)
    {
        pickedup = context.action.IsPressed();
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        thrown = context.action.IsPressed();
    }

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

        PickupAndThrow();
    }
    // Note: Regarding the new input system, for single action input events (ONE BUTTON PRESS = ONE ACTION EVENT),
    // after the event is performed that actions relative bolean variable must be reset to "false" to avoid multiple
    // actions being performed from one button press.
    
    // This should NOT be the case with actions that require holding down buttons like movement
    void PickupAndThrow()
    {
        // Input variable
        if (pickedup)
        {
            if (currentObject)
            {
                // Drop object
                currentObject.GetComponent<VelocityDamager>().Drop();
                currentObject.useGravity = true;
                currentObject = null;
            }
            else
            {
                // Pickup object
                Ray cameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                if (Physics.Raycast(cameraRay, out RaycastHit hitInfo, pickupRange, pickupMask))
                {
                    if (!hitInfo.rigidbody.GetComponent<VelocityDamager>().IsHeld())
                    {
                        currentObject = hitInfo.rigidbody;
                        currentObject.GetComponent<VelocityDamager>().Pickup(gameObject);
                        currentObject.useGravity = false;
                    }
                }
            }

            // Input boolean variable must be reset to false from one button press
            // to emulate the functionality of the old input system
            pickedup = false;
            return;
        }

        // Input variable
        if (thrown && currentObject)
        {
            // Throw object
            currentObject.GetComponent<VelocityDamager>().Drop();
            currentObject.useGravity = true;
            currentObject.AddForce(pickupTarget.forward * throwPower, ForceMode.Impulse);
            currentObject = null;

            thrown = false; // Avoids multiple actions from one input
            return;
        }
    }

    void FixedUpdate()
    {
        // Makes current object travel to the pick up point of the player
        if (currentObject)
        {
            Vector3 directionToPoint = pickupTarget.position - currentObject.position;
            // Not sure how to do it with the selected objects center of mass
            //Vector3 directionToPoint = pickupTarget.position - currentObject.GetComponent<Rigidbody>().centerOfMass;
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
