using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPickup : MonoBehaviour
{
    [SerializeField] private LayerMask pickupMask;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform pickupTarget;
    [Space]
    [SerializeField] private float pickupRange;
    private Rigidbody currentObject;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetCamera(Camera cam)
    {
        playerCamera = cam;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Pickup"))
        {
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
    }

    void FixedUpdate()
    {
        if (currentObject)
        {
            Vector3 directionToPoint = pickupTarget.position - currentObject.position;
            float distanceToPoint = directionToPoint.magnitude;

            currentObject.velocity = directionToPoint * distanceToPoint * 12f;
        }
    }
}
