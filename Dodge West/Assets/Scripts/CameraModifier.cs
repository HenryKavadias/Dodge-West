using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CameraFollow))]
public class CameraModifier : MonoBehaviour
{
    public Camera cam;

    private GameObject player = null;

    public void SetPlayer(GameObject p)
    {
        player = p;
        ModifyCamera(player.GetComponent<PlayerID>().Get());
    }

    private void ModifyCamera(int newID)
    {
        //if (newID != 0)
        //{
        //    //Debug.Log("Player " + newID + " camera has been modified.");
        //}
        

        switch (newID)
        {
            case 0:
                //Debug.Log("No change to ID");

                break;
            case 1:
                //Debug.Log("Player " + newID + " camera has been modified.");
                // X, Y, Width, Height
                cam.rect = new Rect(0f, 0f, 0.5f, 1f);

                break;

            case 2:
                //Debug.Log("Player " + newID + " camera has been modified.");
                // X, Y, Width, Height
                cam.rect = new Rect(0.5f, 0f, 0.5f, 1f);

                break;
            default:
                break;
        }
    }
}
