using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Respawns the object where they initially started
public class ReSpawner : MonoBehaviour
{
    public float deathDelay = 3f;
    public GameObject playerModel;

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
        StartCoroutine(ReturnToSpawn());
    }

    private IEnumerator ReturnToSpawn()
    {
        // Disable player controls and UI temperarily, also empty their objects
        gameObject.GetComponent<PhysicsPickup>().DropObject();
        gameObject.GetComponent<CameraManager>().DisableUI();
        // Disables player controls. It's important to disable the First Person Movement
        // script because features in it frequently turn on and off the UseGravity variable
        // attached to the Rigidbody. Gravity for this object needs to remain off while the
        // respawn is in progress.
        gameObject.GetComponent<PlayerInputHandler>().DisableControls();
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().useGravity = false;

        Spawnbody();

        playerModel.SetActive(false);

        yield return new WaitForSeconds(deathDelay);

        // Need to disable the player object while reseting its position, otherwise it won't
        gameObject.SetActive(false);

        // Set the position of the player
        gameObject.GetComponent<Transform>().position = spawnPosition;

        // Be sure to re-enable player after changing its position
        gameObject.SetActive(true);
        // Enable player model and gravity on object
        playerModel.SetActive(true);

        gameObject.GetComponent<Rigidbody>().useGravity = true;

        // Re-enable player UI and controls
        gameObject.GetComponent<CameraManager>().EnableUI();
        gameObject.GetComponent<PlayerInputHandler>().EnsableControls();

        yield return null;
    }

    public GameObject playerCorpse;

    // Spawns the players body as a pickupable object 
    private void Spawnbody()
    {
        // Find and copy the players material onto the copy of their body
        Object[] matResources = Resources.LoadAll("Materials", typeof(Material));

        Material foundMat = null;
        
        Material playerMat = playerModel.GetComponent<MeshRenderer>().material;

        foreach (Object obj in matResources)
        {
            Material current = (Material)obj;

            if (playerMat.name.ToString().Contains(current.name))
            {
                foundMat = new Material(current);
                break;
            }
        }
        
        // Spawn their copy of their body
        GameObject body = Instantiate(playerCorpse, gameObject.transform.position, gameObject.transform.rotation);

        if (foundMat != null)
        {
            // might need to be modified for more complex player models
            body.transform.GetChild(0).GetComponent<MeshRenderer>().material = foundMat;
        }
    }

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
}

