using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Manages the players ability to pickup objects in the game
// (Objects are picked up with physics)
public class PhysicsPickup : MonoBehaviour
{
    // Variables for pickup ability

    // Variables for Loadsystem
    public List<GameObject> loadInventory = new List<GameObject>();
    // Trasparent pickup variables
    [SerializeField] private bool transparentPickup = false;
    [Range(0f, 1f)]
    [SerializeField] private float transparencyRatio = 0.5f;

    [SerializeField] private LayerMask pickupMask;  // Layer(s) players can pickup objects on
    private Camera playerCamera;                    // Reference to player camera
    [SerializeField] private Transform pickupTarget;// Point where the picked up object will travel to
    [Space]
    [SerializeField] private float pickupRange;     // Distance that players can pick up an object

    // Variables that control the speed in which pickedup objects travel to the "pickupTarget"
    [SerializeField] private float objectTrackingSpeedModifier = 12f;
    [SerializeField] private float maxObjectSpeed = 20f;

    // Controls the force applied to the an object when thrown
    [SerializeField] private float flatThrowPowerPerUnit = 20f; // Power per units of object mass
    [SerializeField] private bool enableThrowPowerWithCap = true;
    [SerializeField] private float minThrowPower = 100f;

    private Rigidbody currentObject;    // Reference to current picked up object

    // Input variables
    private bool pickedup = false;
    private bool thrown = false;
    private bool loadedItem = false;

    // Input functions (OnPickup is used for pick and drop for an object)
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

    // Set Camera reference
    public void SetCamera(Camera cam)
    {
        playerCamera = cam;
    }
    
    void Update()
    {
        // Highlight the object that is being looked at if it's not being held
        HighlightObject();

        // When picking up or dropping an object you must modifiy 3 of it attributes:
        // - It's Velocity damager script, tell it if it's pickup or dropped
        // - Use gravity if dropped, don't use if picked up
        // - the reference of "currentObject", null if dropped, set reference to picked up object
        PickupAndThrow();
    }

    // Holds a reference for the object currently being highlighted
    private GameObject currentHighlight = null;

    // Highlights pickup able objects using an outline script attached to the pickup able objects
    void HighlightObject()
    {
        // If not currently holding an object
        if (currentObject == null)
        {
            // Check if player is looking at a pickup able object
            Ray cameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(cameraRay, out RaycastHit hitInfo, pickupRange, pickupMask))
            {
                GameObject target = hitInfo.rigidbody.gameObject;

                // Check if object is held by another player and has the outline script
                if (!target.GetComponent<VelocityDamager>().IsHeld() && target.GetComponent<Outline>())
                {
                    // Check if highlighted object has changed
                    if (currentHighlight != target && !target.GetComponent<Outline>().enabled)
                    {
                        // If it has, unhighlight old object and highlight new one
                        
                        target.GetComponent<Outline>().enabled = true;

                        if (currentHighlight != null)
                        {
                            currentHighlight.GetComponent<Outline>().enabled = false;
                        }

                        currentHighlight = target;
                    }
                }
            }
            else if (currentHighlight != null)
            {
                // If not looking at an object and current highlighted object isn't null,
                // turn off outline script on object and reset variable
                currentHighlight.GetComponent<Outline>().enabled = false;
                currentHighlight = null;
            }
        }
        else if (currentHighlight != null)
        {
            // If not looking at an object and current highlighted object isn't null,
            // turn off outline script on object and reset variable
            currentHighlight.GetComponent<Outline>().enabled = false;
            currentHighlight = null;
        }
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
            RestoreToOriginalMaterials();
            currentObject = null;
        }
    }

    // Used to balance object throw power
    float DynamicForceToObject()
    {
        // Throw power ideal guide:

        // Power per Units - ppu
        // CAP ppu beyond this point
        // mass 1 - 100 ppu
        // mass 5 - 50 ppu
        // mass 10 - 35 ppu
        // mass 20 - 25 ppu
        // mass 100 - 20 ppu
        // CAP ppu beyond this point

        // Using all data points for a power function gives:
        // y = 89.206x^-0.362

        // Modified version from 80 to 1
        // y = 107.38x^-0.98

        float result = 0;

        // Cap the force power for objects lighter than 1 mass
        if (currentObject.mass < 1)
        {
            result = minThrowPower * currentObject.mass;
            return result;
        }

        // Calculates the throw force value for current object
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

    // Manages the players ability to pickup, drop, and throw objects
    void PickupAndThrow()
    {
        // Can only throw/drop an object if the player is carrying one
        // Can only pickup an object if the player is looking at an object on the
        // pickup layer, is in range, and isn't already holding an object
        
        // Throw functionality must be put before pickup/drop functionality
        if (thrown)
        {
            if (currentObject)
            {
                // Throw object
                currentObject.GetComponent<VelocityDamager>().Drop(true);
                currentObject.useGravity = true;
                currentObject.AddForce(pickupTarget.forward * DynamicForceToObject(), ForceMode.Impulse);

                // Restore to original material state
                RestoreToOriginalMaterials();

                currentObject = null;

                // Avoids multiple actions from one input
                pickedup = false;
                thrown = false;
                return;
            }
            else if (loadInventory.Count > 0)
            {
                GameObject thrownObj = loadInventory[0];
                thrownObj.SetActive(true);
                // Throw object
                thrownObj.GetComponent<VelocityDamager>().Drop(true);
                thrownObj.GetComponent<Rigidbody>().useGravity = true;
                thrownObj.GetComponent<Rigidbody>().AddForce(pickupTarget.forward * DynamicForceToObject(), ForceMode.Impulse);

                thrownObj = null;

                loadInventory.RemoveAt(0);

                // Avoids multiple actions from one input
                pickedup = false;
                thrown = false;
                return;
            }
            
        }

        if (pickedup || thrown)
        {
            if (currentObject)
            {
                // Drop object
                currentObject.GetComponent<VelocityDamager>().Drop();
                currentObject.useGravity = true;

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
                    if (!hitInfo.rigidbody.GetComponent<VelocityDamager>().IsHeld())
                    {
                        currentObject = hitInfo.rigidbody;
                        currentObject.GetComponent<VelocityDamager>().Pickup(gameObject);
                        currentObject.useGravity = false;

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

        // Note: load system still needs implementing
        if (loadedItem)
        {

            
            Ray cameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(cameraRay, out RaycastHit hitInfo, pickupRange, pickupMask))
            {
                if (!hitInfo.rigidbody.GetComponent<VelocityDamager>().IsHeld())
                {
                    //currentObject = hitInfo.rigidbody;
                    //currentObject.GetComponent<VelocityDamager>().Pickup(gameObject);
                    GameObject loadedObject = hitInfo.rigidbody.gameObject;
                    loadInventory.Add(loadedObject);
                    loadedObject.SetActive(false);
                    
                }
            }

            loadedItem = false;
            return;
        }
    }

    // System for pickupable objects with multiple models
    // NOTE: all materials must be added to the Resources/Materials file location

    // Object model material variables
    private List<Material> objectMaterials; // Copies all the materials attached to an object model structure
    private int objectMaterialCount = 0;    // Number of materials attached to object
    private int objectMaterialCounter = 0;  // Current material being reset in object model structure

    private Object[] materialResources; // Materials list from resources

    // Resets object model material variables
    void ResetMaterialVariables()
    {
        objectMaterials = new List<Material>();
        objectMaterialCount = 0;
        objectMaterialCounter = 0;

        // Reset array. Avoids holding large amounts of data
        materialResources = new Object[0];
    }

    // Make object materials transparent and save their previous materials and structure
    void MakeObjectsTransparent()
    {
        if (transparentPickup && currentObject)
        {
            ResetMaterialVariables();

            // If object has children, start the relevent recursion function
            if (currentObject.gameObject.transform.childCount > 0)
            {
                // Get material from resources folder, its here to reduce calls
                materialResources = Resources.LoadAll("Materials", typeof(Material));

                ModifyObjectMaterial(currentObject.gameObject.transform);
            }
        }
    }

    // Uses a recursion function to dynamically apply the material change to a
    // pickupable object no matter what object structure it has
    void ModifyObjectMaterial(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Check to see if object has a model
            if (child.gameObject.GetComponent<MeshRenderer>() != null)
            {
                GameObject curObj = child.gameObject;

                // Get material of current object
                Material mat = curObj.GetComponent<MeshRenderer>().material;
                
                bool foundMaterial = false;

                // Search for the material of current object
                foreach (Object obj in materialResources)
                {
                    Material curMat = (Material)obj;

                    // If material is found in the array of materials...
                    if (mat.name.ToString().Contains(curMat.name))
                    {
                        // Add a copy of that material to the object material reference list
                        objectMaterials.Add(new Material(curMat));
                        foundMaterial = true;
                        break;
                    }
                }

                // Error if material isn't in resources folder
                if (!foundMaterial)
                {
                    objectMaterials.Add(new Material(mat));
                    Debug.Log("WARNING, material hasn't been added to " +
                        "Resources/Materials, please correct");
                }

                // Makes object transparent
                MakeMaterialTransparent(mat);
            }

            // If the current object has children, trigger recursion will passing in current object
            if (child.childCount > 0)
            {
                ModifyObjectMaterial(child);
            }
        }
    }

    // Make object transparent
    void MakeMaterialTransparent(Material mat)
    {
        // Makes object transparent (render mode)
        mat = StandardShaderUtils.ChangeRenderMode(
            mat, StandardShaderUtils.BlendMode.Transparent);

        // Alter the transparency (alpha value) of the object
        Color color = mat.color;
        color.a = transparencyRatio;
        mat.color = color;
    }

    // Restores object materials to their previous states
    void RestoreToOriginalMaterials()
    {
        if (transparentPickup && currentObject)
        {
            // If object has children, start the relevent recursion function
            if (currentObject.gameObject.transform.childCount > 0)
            {
                objectMaterialCount = objectMaterials.Count;
                ResetObjectMaterial(currentObject.gameObject.transform);

                ResetMaterialVariables();
            }
        }
    }

    // Uses a recursion function to dynamically restores the material previous material states
    // to a pickupable object no matter what object structure it has
    void ResetObjectMaterial(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Check to see if object has a model, and if their are still materails to revert
            if (child.gameObject.GetComponent<MeshRenderer>() != null && 
                objectMaterialCounter < objectMaterialCount)
            {
                GameObject curObj = child.gameObject;

                // When re-applying the old material, find a way to get its copy from the assets folder

                // Re-apply object material stored in the materials list
                curObj.GetComponent<MeshRenderer>().material = objectMaterials[objectMaterialCounter];

                objectMaterialCounter++;

            }
            else if (objectMaterialCounter >= objectMaterialCount)
            {
                return;
            }

            // If the current object has children, trigger recursion will passing in current object
            if (child.childCount > 0)
            {
                ResetObjectMaterial(child);
            }
        }
    }

    // Controls the physics behind picking up an object
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
