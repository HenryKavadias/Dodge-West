using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicListView : MonoBehaviour
{
    public GameObject listItemprefab;
    public Transform listItemHolder;
    public int numOfListItems;

    private void Start()
    {
        for (int i = 0; i < numOfListItems; i++)
        {
            Instantiate(listItemprefab, listItemHolder);
        }
    }
}
