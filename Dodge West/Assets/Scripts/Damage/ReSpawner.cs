using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Respawns the object where they initially started
public class ReSpawner : MonoBehaviour
{
    Vector3 spawnPosition = Vector3.zero;

    // Set the spawn position where they start in the scene
    void Start()
    {
        SetSpawnPosition(gameObject.GetComponent<Transform>().position);
    }

    // Set spawn position
    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
    }

    // Return the object back to their spawn position
    public void BackToSpawn()
    {
        // Need to disable the player object while reseting its position, otherwise it won't
        gameObject.SetActive(false);
        
        // Set the position of the player
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Transform>().position = spawnPosition;
        //Debug.Log(spawnPosition + " " + gameObject.GetComponent<Transform>().position);

        // Be sure to re-enable player after changing its position
        gameObject.SetActive(true);
    }
}
