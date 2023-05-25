using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsPickup : MonoBehaviour
{
    [SerializeField] private bool transparentPickup = false;
    [Range(0f, 1f)]
    [SerializeField] private float transparencyRatio = 0.5f;

    [SerializeField] private LayerMask pickupMask;
    private Camera playerCamera;
    [SerializeField] private Transform pickupTarget;
    [Space]
    [SerializeField] private float pickupRange;
    [SerializeField] private float objectTrackingSpeedModifier = 12f;
    [SerializeField] private float maxObjectSpeed = 20f;
    [SerializeField] private float flatThrowPowerPerUnit = 20f; // Power per units of object mass
    [SerializeField] private bool enableThrowPowerWithCap = true;
    private Rigidbody currentObject;

    private bool pickedup = false;
    private bool thrown = false;
    private bool loadedItem = false;

    public void OnPickup(InputAction.CallbackContext context)
    {
        pickedup = context.action.IsPressed();
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        thrown = context.action.IsPressed();
    }

    public void OnLoadItem(InputAction.CallbackContext context)
    {
        loadedItem = context.action.IsPressed();
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
    public void DropObject()
    {
        if (currentObject)
        {
            // Drop object
            currentObject.GetComponent<VelocityDamager>().Drop();
            currentObject.useGravity = true;
            currentObject = null;
        }
    }

    // Used to balance object throw power
    float DynamicForceToObject()
    {
        // Throw power ideal guide:

        // Power per Units - ppu
        // CAP ppu
        // mass 1 - 100 ppu
        // mass 5 - 50 ppu
        // mass 10 - 35 ppu
        // mass 20 - 25 ppu
        // mass 100 - 20 ppu
        // CAP ppu

        // Using all data points for a power function gives:
        // y = 89.206x^-0.362

        // Modified version from 80 to 1
        // y = 107.38x^-0.98

        float result = 0;

        if (currentObject.mass < 1)
        {
            result = 100 * currentObject.mass;
            return result;
        }


        if (enableThrowPowerWithCap)
        {
            // 80ppu to 1ppu with a flat value
            float powerPerUnits = 107.38f * Mathf.Pow(currentObject.mass, -0.98f);

            result = (powerPerUnits + flatThrowPowerPerUnit) * currentObject.mass;
        }
        else
        {
            // 100ppu to 20ppu without a flat value
            float powerPerUnits = 89.206f * Mathf.Pow(currentObject.mass, -0.362f);

            result = powerPerUnits * currentObject.mass;
        }


        return result;
    }

    // This should NOT be the case with actions that require holding down buttons like movement
    void PickupAndThrow()
    {
        // Input variable
        if (thrown && currentObject)
        {
            // Throw object
            currentObject.GetComponent<VelocityDamager>().Drop(true);
            currentObject.useGravity = true;

            //Debug.Log("Force applied: " + DynamicForceToObject() +
            //        ", Mass of Object: " + currentObject.mass);
            currentObject.AddForce(pickupTarget.forward * DynamicForceToObject(), ForceMode.Impulse);

            ResetMaterial();

            currentObject = null;

            pickedup = false;
            thrown = false; // Avoids multiple actions from one input
            return;
        }

        // Input variable
        if (pickedup || thrown)
        {
            if (currentObject)
            {
                // Drop object
                currentObject.GetComponent<VelocityDamager>().Drop();
                currentObject.useGravity = true;

                ResetMaterial();

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

                        MakeTransparent();
                    }
                }
            }

            // Input boolean variable must be reset to false from one button press
            // to emulate the functionality of the old input system
            thrown = false;
            pickedup = false;
            return;
        }

        if (loadedItem)
        {

            loadedItem = false;
            return;
        }
    }

    // Make picked up object transparent (so it doesn't block the players view)
    void MakeTransparent()
    {
        if (transparentPickup && currentObject)
        {
            GameObject curObj = currentObject.gameObject;

            Material mat = curObj.transform.GetChild(0).
                gameObject.GetComponent<MeshRenderer>().material;

            mat = StandardShaderUtils.ChangeRenderMode(
                mat, StandardShaderUtils.BlendMode.Transparent);

            Color color = mat.color;
            color.a = transparencyRatio;
            mat.color = color;
        }
    }

    // Revert the picked up object to the render mode they were before
    void ResetMaterial()
    {
        if (transparentPickup && currentObject)
        {
            GameObject curObj = currentObject.gameObject;

            Material mat = curObj.transform.GetChild(0).
                gameObject.GetComponent<MeshRenderer>().material;

            mat = StandardShaderUtils.ChangeRenderMode(
                mat, StandardShaderUtils.BlendMode.Opaque);

            Color color = mat.color;
            color.a = 1f;
            mat.color = color;
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
