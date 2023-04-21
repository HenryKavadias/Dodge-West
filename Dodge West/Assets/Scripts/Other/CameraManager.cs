using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(MouseLook), typeof(PhysicsPickup))]
public class CameraManager : MonoBehaviour
{
    [Header("Camera Object")]
    public GameObject cameraObject;

    // Start is called before the first frame update
    void Start()
    {
        if (cameraObject != null && GetComponent<MouseLook>().cam != null)
        {
            GameObject camTemp = Instantiate(cameraObject);

            camTemp.GetComponent<CameraModifier>().SetPlayer(gameObject);

            camTemp.GetComponent<CameraFollow>().SetTarget(GetComponent<MouseLook>().cam);

            // Camera needs to be the first child object
            Camera camReal = camTemp.transform.GetChild(0).GetComponent<Camera>();
            if (camReal != null)
            {
                // set camera to the pickup script
                gameObject.GetComponent<PhysicsPickup>().SetCamera(camReal);

                // The code below may be unnecessary

                // Set camera to player input component
                if (gameObject.GetComponent<PlayerInput>())
                {
                    gameObject.GetComponent<PlayerInput>().camera = camReal;
                }

            }
            else
            {
                Debug.Log("Camera object has no object or compenent/objects are ill-positioned");
            }
        }
        else
        {
            Debug.Log("Player is without camera!!!");
        }
    }
}
