using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            //if (xAngle >= 0 && xAngle <= lowerRotationCap)
            //{
            //    xRotation = Mathf.Clamp(
            //    matchObject.eulerAngles.x,
            //    0f,
            //    lowerRotationCap);
            //}
            //else if (xAngle >= (360f - upperRotationCap) && xAngle <= 360f)
            //{
            //    xRotation = Mathf.Clamp(
            //    matchObject.eulerAngles.x,
            //    (360f - upperRotationCap),
            //    360f);
            //}

            //Debug.Log(matchObject.eulerAngles.x);

            gameObject.transform.rotation = 
                Quaternion.Euler(
                xRotation, //matchObject.eulerAngles.x, 
                matchObject.eulerAngles.y, 0f);
        }
    }
}
