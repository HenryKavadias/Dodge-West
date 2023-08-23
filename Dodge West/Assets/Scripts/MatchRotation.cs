using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    public Transform matchObject = null;

    // Update is called once per frame
    void Update()
    {
        if (matchObject)
        {
            gameObject.transform.rotation = Quaternion.Euler(
                matchObject.eulerAngles.x, matchObject.eulerAngles.y, 0f);
            //matchObject.rotation.y, 
            //matchObject.rotation.z);
            //Debug.Log("Change");
        }
    }
}
