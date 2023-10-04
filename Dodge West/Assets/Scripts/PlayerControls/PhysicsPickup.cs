using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

// Manages the players ability to pickup objects in the game
// (Objects are picked up with physics)
public class PhysicsPickup : MonoBehaviour
{
    // Variables for Loadsystem
    private List<GameObject> loadInventory = new List<GameObject>();

    // Trasparent pickup variables
    [SerializeField] private bool transparentPickup = false;
    [Range(0f, 1f)]
    [SerializeField] private float transparencyRatio = 0.5f;

    [SerializeField] private LayerMask pickupMask;  // Layer(s) players can pickup objects on
    private Camera playerCamera;                    // Reference to player camera
    [SerializeField] private Transform pickupTarget;// Point where the picked up object will travel to
    [SerializeField] private Transform largePickupTarget;
    [Space]
    [SerializeField] private float pickupRange;     // Distance that players can pick up an object

    // Variables that control the speed in which pickedup objects travel to the "pickupTarget"
    [SerializeField] private float objectTrackingSpeedModifier = 12f;
    [SerializeField] private float maxObjectSpeed = 20f;

    // Controls the force applied to the an object when thrown
    [SerializeField] private float flatThrowPowerPerUnit = 20f; // Power per units of object mass
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
        // Update load positio
        UpdateLoadPosition();

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

    public bool enableHighlight = true;
    // Highlights pickup able objects using an outline script attached to the pickup able objects
    void HighlightObject()
    {
        if (!enableHighlight)
        {
            return;
        }
        // If not currently holding an object
        if (currentObject == null && playerCamera)
        {
            // Check if player is looking at a pickup able object
            Ray cameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(cameraRay, out RaycastHit hitInfo, pickupRange, pickupMask))
            {
                GameObject target = hitInfo.rigidbody.gameObject;
                //Debug.Log("Contact");
                // Check if object isn't held by another player and has the outline script
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

        // Empty player loaded inventory
        EmptyLoadInventory();
        gameObject.GetComponent<CameraManager>().EmptyInventoryUI();
    }

    // Used to balance object throw power
    float DynamicForceToObject(float objMass, bool loaded = false)
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
        if (objMass < 1)
        {
            result = (minThrowPower + GetComponent<Rigidbody>().velocity.magnitude) * 
                objMass;
            return result;
        }

        // Calculates the throw force value for current object

        // 80ppu to 1ppu with a flat value
        float powerPerUnits = 107.38f * Mathf.Pow(objMass, -0.98f);

        if (loaded)
        {
            result = (powerPerUnits + flatThrowPowerPerUnit +
                GetComponent<Rigidbody>().velocity.magnitude) * objMass;
        }
        else
        {
            result = (powerPerUnits + flatThrowPowerPerUnit) * objMass;
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
                ThrowObject(true);
                

                // Avoids multiple actions from one input
                pickedup = false;
                thrown = false;
                loadedItem = false;
                return;
            }
            else if (loadInventory.Count > 0)
            {
                ThrowObject();

                // Avoids multiple actions from one input
                pickedup = false;
                thrown = false;
                loadedItem = false;
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
                if (Physics.Raycast(cameraRay, out RaycastHit hitInfo, pickupRange, pickupMask) )
                {
                    if (!hitInfo.rigidbody.GetComponent<VelocityDamager>().IsHeld() 
                        && hitInfo.rigidbody.gameObject.tag != "Player")
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
            loadedItem = false;
            return;
        }

        // Ability for the player to load objects into their invetory to throw when they aren't holding another object.
        // These "loaded" objects are added to the players inventory and disabled in the game world.
        // They can then throw these objects. When they do, the object is reposition infront of the player, enabled,
        // then the throw force is applied to it
        if (loadedItem)
        {
            // Load the object you are holding
            if (currentObject)
            {
                GameObject loadedObject = currentObject.gameObject;

                if (loadableRestriction)
                {
                    if (loadedObject.GetComponent<VelocityDamager>().loadable)
                    {
                        if (massLimitEnabled || itemLimitEnabled)
                        {
                            if (LoadLimitCheck(loadedObject.GetComponent<Rigidbody>().mass))
                            {
                                LoadItem(loadedObject, true);
                            }
                            else
                            {
                                // Put warning indication here telling the player they can't load any more
                            }
                        }
                        else
                        {
                            LoadItem(loadedObject, true);
                        }
                    }
                    else
                    {
                        Debug.Log("Object isn't loadable");
                    }
                }
                else
                {
                    if (massLimitEnabled || itemLimitEnabled)
                    {
                        if (LoadLimitCheck(loadedObject.GetComponent<Rigidbody>().mass))
                        {
                            LoadItem(loadedObject, true);
                        }
                        else
                        {
                            // Put warning indication here telling the player they can't load any more
                        }
                    }
                    else
                    {
                        LoadItem(loadedObject, true);
                    }
                }
            }
            else
            {
                // Load the object you are looking at

                Ray cameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                if (Physics.Raycast(cameraRay, out RaycastHit hitInfo, pickupRange, pickupMask))
                {
                    if (!hitInfo.rigidbody.GetComponent<VelocityDamager>().IsHeld())
                    {
                        GameObject loadedObject = hitInfo.rigidbody.gameObject;

                        if (loadableRestriction)
                        {
                            if (loadedObject.GetComponent<VelocityDamager>().loadable)
                            {
                                if (massLimitEnabled || itemLimitEnabled)
                                {
                                    if (LoadLimitCheck(loadedObject.GetComponent<Rigidbody>().mass))
                                    {
                                        LoadItem(loadedObject);
                                    }
                                    else
                                    {
                                        // Put warning indication here telling the player they can't load any more
                                    }
                                }
                                else
                                {
                                    // No limitations
                                    LoadItem(loadedObject);
                                }
                            }
                            else
                            {
                                Debug.Log("Object isn't loadable");
                            }
                        }
                        else
                        {
                            if (massLimitEnabled || itemLimitEnabled)
                            {
                                if (LoadLimitCheck(loadedObject.GetComponent<Rigidbody>().mass))
                                {
                                    LoadItem(loadedObject);
                                }
                                else
                                {
                                    // Put warning indication here telling the player they can't load any more
                                }
                            }
                            else
                            {
                                // No limitations
                                LoadItem(loadedObject);
                            }
                        }
                    }
                }
            }

            thrown = false;
            pickedup = false;
            loadedItem = false;
            return;
        }
    }

    public UnityEvent throwEffect;

    void ThrowObject(bool current = false)
    {
        if (current)
        {
            // Throw object
            currentObject.GetComponent<VelocityDamager>().Drop(true);
            currentObject.useGravity = true;
            currentObject.AddForce(
                pickupTarget.forward *
                DynamicForceToObject(currentObject.mass),
                ForceMode.Impulse);

            // Restore to original material state
            RestoreToOriginalMaterials();

            throwEffect?.Invoke();

            currentObject = null;
        }
        else
        {
            GameObject thrownObj = loadInventory[0];

            thrownObj.SetActive(false);

            SubtractMassFromLoad(thrownObj.GetComponent<Rigidbody>().mass);

            thrownObj.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // Throw object
            thrownObj.GetComponent<VelocityDamager>().Drop(true);
            thrownObj.GetComponent<Rigidbody>().useGravity = true;
            thrownObj.GetComponent<Transform>().position = pickupTarget.position;

            thrownObj.SetActive(true);
            thrownObj.GetComponent<Rigidbody>().AddForce(pickupTarget.forward *
                DynamicForceToObject(thrownObj.GetComponent<Rigidbody>().mass, true), 
                ForceMode.Impulse);

            ToggleCollider(thrownObj.transform, true);
            thrownObj.transform.parent = null;
            //thrownObj.SetActive(true);

            thrownObj = null;

            loadInventory.RemoveAt(0);

            if (visualLoad)
            {
                UpdateVisualLoad(true);
            }

            // Update UI 
            gameObject.GetComponent<CameraManager>().UpdateInventoryUI();

            throwEffect?.Invoke();
        }
    }

    public GameObject loadPositionSmall = null;
    public GameObject loadPositionMid = null;
    public GameObject loadPositionFar = null;

    public GameObject preLoadedObject = null;

    public bool visualLoad = false;

    void SetPreLoadedItem()
    {
        if (preLoadedObject)
        {
            visualLoad = true;

            GameObject item = Instantiate(preLoadedObject);

            LoadItem(item);

            visualLoad = false;
        }
    }

    // Load inventory check variables
    public bool massLimitEnabled = true;
    public bool itemLimitEnabled = true;
    public bool loadableRestriction = true;

    public int loadItemLimit = 50;
    public float loadMassLimit = 500;
    private float currentLoad = 0;

    void ToggleCollider(Transform item, bool active = false)
    {
        foreach (Transform obj in item)
        {
            if (obj.gameObject.GetComponent<Collider>() != null)
            {
                obj.gameObject.GetComponent<Collider>().enabled = active;
            }

            if (obj.childCount > 0)
            {
                ToggleCollider(obj, active);
            }
        }
    }

    private void Awake()
    {
        if (preLoadedObject != null)
        {
            Invoke(nameof(SetPreLoadedItem), 0.5f);
        }

        // Get material from resources folder, its here to reduce calls
        //materialResources = Resources.LoadAll("Materials", typeof(Material));
    }

    // Use after adding or removing an item
    void UpdateVisualLoad(bool thrown = false)
    {     
        if (loadInventory.Count > 0)
        {
            if (loadInventory.Count == 1 || thrown)
            {
                ToggleCollider(loadInventory[0].transform, false);
                loadInventory[0].SetActive(true);

                if (loadInventory.Count > 1)
                {
                    ToggleCollider(loadInventory[1].transform, true);
                    //loadInventory[1].transform.parent = null;
                    loadInventory[1].SetActive(false);
                }
            }
        }
    }

    void UpdateLoadPosition()
    {
        if (loadInventory.Count > 0)
        {
            SetLoadPosition(loadInventory[0].GetComponent<VelocityDamager>().loadDistance);
            //loadInventory[0].transform.rotation = loadPosition.transform.rotation;
        }
    }

    //public Vector3 loadDistanceModifier = new Vector3 (0, 0, 0);
    //public float largeLoadModifer = 1.5f;
    void SetLoadPosition(LoadDistance type)
    {
        //Vector3 ogPosition = loadPosition.transform.position;

        switch (type)
        {
            case LoadDistance.Close:
                loadInventory[0].transform.position =
                    loadPositionSmall.transform.position;
                loadInventory[0].transform.rotation = 
                    loadPositionSmall.transform.rotation;
                break;
            case LoadDistance.Mid:
                loadInventory[0].transform.position =
                    loadPositionMid.transform.position;
                loadInventory[0].transform.rotation =
                    loadPositionMid.transform.rotation;
                break;
            case LoadDistance.Far:
                loadInventory[0].transform.position =
                    loadPositionFar.transform.position;
                loadInventory[0].transform.rotation =
                    loadPositionFar.transform.rotation;
                break;
            default:
                loadInventory[0].transform.position =
                    loadPositionSmall.transform.position;
                loadInventory[0].transform.rotation =
                    loadPositionSmall.transform.rotation;
                break;
        }
    }

    public UnityEvent loadEffect;

    void LoadItem(GameObject item, bool held = false)
    {
        if (held)
        {
            // Restore to original material state
            RestoreToOriginalMaterials();

            item.GetComponent<VelocityDamager>().Pickup(gameObject);
            loadInventory.Add(item);
            AddMassToLoad(item.GetComponent<Rigidbody>().mass);

            item.SetActive(false);

            if (visualLoad)
            {
                UpdateVisualLoad();
            }

            // Update UI
            gameObject.GetComponent<CameraManager>().UpdateInventoryUI(
                true, item.GetComponent<VelocityDamager>().objIcon);

            loadEffect?.Invoke();

            currentObject = null;
        }
        else
        {
            item.GetComponent<VelocityDamager>().Pickup(gameObject);
            loadInventory.Add(item);
            AddMassToLoad(item.GetComponent<Rigidbody>().mass);
            item.GetComponent<Rigidbody>().useGravity = false;
            item.SetActive(false);

            if (visualLoad)
            {
                UpdateVisualLoad();
            }

            loadEffect?.Invoke();

            // Update UI
            gameObject.GetComponent<CameraManager>().UpdateInventoryUI(
                true, item.GetComponent<VelocityDamager>().objIcon);
        }
    }

    // Empty players inventory and reset relavent variables
    void EmptyLoadInventory()
    {
        // Undo first item carry
        if (loadInventory.Count > 0) 
        {
            ToggleCollider(loadInventory[0].transform, true);
            loadInventory[0].transform.parent = null;
            loadInventory[0].SetActive(false);
        }

        foreach (GameObject obj in loadInventory)
        {
            ToggleCollider(obj.transform, true);
            obj.transform.parent = null;
            //obj.SetActive(true);

            // Reposition object on player hold point. This might go wrong
            obj.GetComponent<Transform>().position = pickupTarget.position;

            obj.SetActive(true);
            obj.GetComponent<VelocityDamager>().Drop();
            obj.GetComponent<Rigidbody>().useGravity = true;
        }
        loadInventory.Clear();
        currentLoad = 0;
    }

    public UnityEvent burdenEffect;

    // Checks if player can load an object into their inventory
    bool LoadLimitCheck(float objMass = 0)
    {
        if (massLimitEnabled && itemLimitEnabled)
        {
            // Both limitations
            if (currentLoad + objMass <= loadMassLimit &&
                loadInventory.Count + 1 <= loadItemLimit)
            {
                return true;
            }

            Debug.Log("Too heavy or not enough space, can't load more.");
        }
        else if (!massLimitEnabled && itemLimitEnabled)
        {
            // Item limitation
            if (loadInventory.Count + 1 <= loadItemLimit)
            {
                return true;
            }

            Debug.Log("Not enough space, can't load more.");
        }
        else if (massLimitEnabled && !itemLimitEnabled)
        {
            // Mass limitation
            if (currentLoad + objMass <= loadMassLimit)
            {
                return true;
            }

            Debug.Log("Too heavy, can't load more.");
        }

        if (massLimitEnabled || itemLimitEnabled)
        {
            burdenEffect?.Invoke();
        }

        return false;
    }

    // Add object mass to current load
    void AddMassToLoad(float mass)
    {
        currentLoad += mass;
    }

    // Subtract object mass to current load
    void SubtractMassFromLoad(float mass)
    {
        currentLoad -= mass;

        if (currentLoad < 0)
        {
            currentLoad = 0;
        }
    }

    // System for pickupable objects with multiple models
    // NOTE: all materials must be added to the Resources/Materials file location

    // Object model material variables
    private List<Material> objectMaterials; // Copies all the materials attached to an object model structure
    private int objectMaterialCount = 0;    // Number of materials attached to object
    private int objectMaterialCounter = 0;  // Current material being reset in object model structure

    private Object[] materialResources; // Materials list from resources

    private void Start()
    {
        // Get material from resources folder, its here to reduce calls
        materialResources = Resources.LoadAll("Materials", typeof(Material));
    }

    // Resets object model material variables
    void ResetMaterialVariables()
    {
        objectMaterials = new List<Material>();
        objectMaterialCount = 0;
        objectMaterialCounter = 0;

        // Reset array. Avoids holding large amounts of data
        //materialResources = new Object[0];
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
                //materialResources = Resources.LoadAll("Materials", typeof(Material));

                ModifyObjectMaterial(currentObject.gameObject.transform);
            }
        }
    }

    // Note: doesn't work for objects with multiple materials on it
    // (see AltPickup script, doesn't yet work)

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

        // Reduce metallic value
        mat.SetFloat("_Metallic", 0.0f);
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
            if (largePickupTarget != null && currentObject.GetComponent<VelocityDamager>().loadDistance == LoadDistance.Far)
            {
                Vector3 directionToPoint = largePickupTarget.position - currentObject.position;
                // Not sure how to do it with the selected objects center of mass
                //Vector3 directionToPoint = pickupTarget.position - currentObject.GetComponent<Rigidbody>().centerOfMass;
                float distanceToPoint = directionToPoint.magnitude;

                currentObject.velocity = directionToPoint * distanceToPoint * objectTrackingSpeedModifier;

                if (currentObject.velocity.magnitude > maxObjectSpeed)
                {
                    currentObject.velocity = Vector3.ClampMagnitude(currentObject.velocity, maxObjectSpeed);
                }
            }
            else
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

            //Mathf.Clamp()
        }

        //if (loadInventory.Count > 0)
        //{
        //    Rigidbody obj = loadInventory[0].GetComponent<Rigidbody>();
            
        //    Vector3 directionToPoint = loadPosition.transform.position - obj.position;
        //    // Not sure how to do it with the selected objects center of mass
        //    //Vector3 directionToPoint = pickupTarget.position - currentObject.GetComponent<Rigidbody>().centerOfMass;
        //    float distanceToPoint = directionToPoint.magnitude;

        //    obj.velocity = directionToPoint * distanceToPoint * objectTrackingSpeedModifier;

        //    if (obj.velocity.magnitude > maxObjectSpeed)
        //    {
        //        obj.velocity = Vector3.ClampMagnitude(obj.velocity, maxObjectSpeed);
        //    }
        //}
    }
}
