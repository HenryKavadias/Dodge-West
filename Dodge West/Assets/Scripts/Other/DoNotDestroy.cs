using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adds one object with specific tag to the don't destroy on load scene
// (currently used for music object for between scene music)
public class DoNotDestroy : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("GameMusic");

        if (musicObj.Length > 1 )
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
