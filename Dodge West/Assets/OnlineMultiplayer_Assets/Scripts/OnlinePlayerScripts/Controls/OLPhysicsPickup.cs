using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

// Manages the players ability to pickup objects in the game
// (Objects are picked up with physics)
public class OLPhysicsPickup : PhysicsPickup
{
    bool allowAction = true;

    public void RefreshInput(bool pickup, bool throwing, bool load)
    {
        // May need to change this in the future
        // depending on how this mechanic works
        loadedItem = load;

        // Prevents repeated actions when the button is held down
        if (!allowAction && (pickup || throwing))
        {
            return;
        }
        else
        {
            allowAction = true;
        }

        if (allowAction)
        {
            if (pickup)
            {
                pickedup = true;
                allowAction = false;
            }
            else if (throwing)
            {
                thrown = true;
                allowAction = false;
            }
        }

    }

    public override void DropObject()
    {
        if (currentObject)
        {
            // Drop object
            currentObject.GetComponent<VelocityDamager>().Drop();
            currentObject.useGravity = true;

            // Network
            currentObject.GetComponent<NetworkPickup>().Drop();

            RestoreToOriginalMaterials();
            currentObject = null;
        }
    }

    protected override void PickupAndThrow()
    {
        // Can only throw/drop an object if the player is carrying one
        // Can only pickup an object if the player is looking at an object on the
        // pickup layer, is in range, and isn't already holding an object

        // Throw functionality must be put before pickup/drop functionality
        if (thrown && currentObject)
        {
            // Throw object
            currentObject.GetComponent<VelocityDamager>().Drop(true);
            currentObject.useGravity = true;
            currentObject.AddForce(pickupTarget.forward * DynamicForceToObject(), ForceMode.Impulse);

            // Network
            currentObject.GetComponent<NetworkPickup>().Drop();

            // Restore to original material state
            RestoreToOriginalMaterials();

            currentObject = null;

            // Avoids multiple actions from one input
            pickedup = false;
            thrown = false;
            return;
        }

        if (pickedup || thrown)
        {
            if (currentObject)
            {
                // Drop object
                currentObject.GetComponent<VelocityDamager>().Drop();
                currentObject.useGravity = true;

                // Network
                currentObject.GetComponent<NetworkPickup>().Drop();

                // Restore to original material state
                RestoreToOriginalMaterials();

                currentObject = null;
            }
            else
            {
                // Pickup object
                Ray cameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

                if (Physics.Raycast(cameraRay, out RaycastHit hitInfo, pickupRange, pickupMask))
                {
                    // Network
                    RaycastHit objHit = GetComponent<RaycastNetwork>().CheckRaycast(cameraRay);

                    if (!objHit.rigidbody.GetComponent<NetworkPickup>().IsHeld() && 
                        !objHit.rigidbody.GetComponent<VelocityDamager>().IsHeld())
                    {
                        currentObject = objHit.rigidbody;
                        currentObject.GetComponent<VelocityDamager>().Pickup(gameObject);
                        currentObject.useGravity = false;

                        Debug.Log(objHit.rigidbody.GetComponent<NetworkPickup>().IsHeld());

                        // Network
                        currentObject.GetComponent<NetworkPickup>().Pickup();

                        MakeObjectsTransparent();
                        //MakeTransparent();
                    }
                }
            }

            // Input boolean variable must be reset to false from one button press
            // to emulate the functionality of the old input system
            thrown = false;
            pickedup = false;
            return;
        }
    }

    public void UpdatePPup()
    {
        PickupAndThrow();

        if (currentObject)
        {
            Vector3 directionToPoint = pickupTarget.position - currentObject.position;
            // Not sure how to do it with the selected objects center of mass
            //Vector3 directionToPoint = pickupTarget.position - currentObject.GetComponent<Rigidbody>().centerOfMass;
            float distanceToPoint = directionToPoint.magnitude;

            // Doesn't work in online mode
            currentObject.velocity = directionToPoint * distanceToPoint * objectTrackingSpeedModifier;

            if (currentObject.velocity.magnitude > maxObjectSpeed)
            {
                currentObject.velocity = Vector3.ClampMagnitude(currentObject.velocity, maxObjectSpeed);
            }

        }
    }
    
    protected override void Update()
    {
        // When picking up or dropping an object you must modifiy 3 of it attributes:
        // - It's Velocity damager script, tell it if it's pickup or dropped
        // - Use gravity if dropped, don't use if picked up
        // - the reference of "currentObject", null if dropped, set reference to picked up object

        //PickupAndThrow();
    }

    // Controls the physics behind picking up an object
    protected override void FixedUpdate()
    {
        // Makes current object travel to the pick up point of the player
        //if (currentObject)
        //{
        //    Vector3 directionToPoint = pickupTarget.position - currentObject.position;
        //    // Not sure how to do it with the selected objects center of mass
        //    //Vector3 directionToPoint = pickupTarget.position - currentObject.GetComponent<Rigidbody>().centerOfMass;
        //    float distanceToPoint = directionToPoint.magnitude;

        //    currentObject.velocity = directionToPoint * distanceToPoint * objectTrackingSpeedModifier;

        //    if (currentObject.velocity.magnitude > maxObjectSpeed)
        //    {
        //        currentObject.velocity = Vector3.ClampMagnitude(currentObject.velocity, maxObjectSpeed);
        //    }

        //}
    }
}
