using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObject : MonoBehaviour
{
    public GameObject obj = null;
    public float timerDelay = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        if (obj != null && !obj.activeSelf)
        {
            Invoke(nameof(enableObject), timerDelay);
        }
    }

    void enableObject()
    {
        obj.SetActive(true);
    }
}
