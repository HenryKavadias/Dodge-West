using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentList : MonoBehaviour
{
    // For objects with many fragements, it is recommened to add all the
    // fragements manually to the list to avoid performance stress
    public List<GameObject> objectFragements = new List<GameObject>();

    private void Awake()
    {
        //if (objectFragements.Count <= 0) 
        //{
        //    foreach (Transform t in transform)
        //    {
        //        objectFragements.Add(t.gameObject);
        //    }
        //}
    }
}
