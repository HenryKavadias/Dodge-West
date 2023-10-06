using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to match the rotation of the players camera
// with their character models head. Camped between two limits 
public class MatchRotation : MonoBehaviour
{
    public Transform matchObject = null;

    public CameraControl control;

    [SerializeField]
    private float upperRotationCap = 70f;
    [SerializeField]
    private float lowerRotationCap = -70f;

    // Update is called once per frame
    void Update()
    {
        if (matchObject && control)
        {
            //float xAngle = matchObject.eulerAngles.x;

            float xRotation = control.GetXRotation();

            xRotation = Mathf.Clamp(
                xRotation,
                lowerRotationCap,
                upperRotationCap);

            gameObject.transform.rotation = 
                Quaternion.Euler(
                xRotation, //matchObject.eulerAngles.x, 
                matchObject.eulerAngles.y, 0f);
        }
    }
}
