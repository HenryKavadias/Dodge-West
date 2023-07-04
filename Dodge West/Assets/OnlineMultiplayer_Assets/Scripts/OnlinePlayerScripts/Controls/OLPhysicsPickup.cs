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
    
    public void UpdatePPup()
    {
        PickupAndThrow();

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
    // Note: Regarding the new input system, for single action input events (ONE BUTTON PRESS = ONE ACTION EVENT),
    // after the event is performed that actions relative bolean variable must be reset to "false" to avoid multiple
    // actions being performed from one button press.
    //public override void DropObject()
    //{
    //    if (currentObject)
    //    {
    //        // Drop object
    //        currentObject.GetComponent<VelocityDamager>().Drop();
    //        currentObject.useGravity = true;
    //        RestoreToOriginalMaterials();
    //        currentObject = null;
    //    }
    //}

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
