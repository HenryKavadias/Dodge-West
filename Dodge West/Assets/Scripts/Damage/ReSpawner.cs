using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawner : MonoBehaviour
{
    Vector3 spawnPosition = Vector3.zero;


    void Start()
    {
        spawnPosition = gameObject.GetComponent<Transform>().position;
        //Debug.Log(spawnPosition);
    }

    public void BackToSpawn()
    {
        // need to disable the player object while reseting its position, otherwise it won't
        gameObject.SetActive(false);
        
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Transform>().position = spawnPosition;
        //Debug.Log(spawnPosition + " " + gameObject.GetComponent<Transform>().position);

        // be sure to re-enable player after changing its position
        gameObject.SetActive(true);
    }
}
