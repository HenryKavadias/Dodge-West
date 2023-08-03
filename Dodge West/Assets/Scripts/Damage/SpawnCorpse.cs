using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCorpse : Spawner
{
    public GameObject playerModel;

    public override void Spawn(GameObject prefab = null)
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

        if (prefabObj)
        {
            // Spawn their copy of their body
            GameObject body = Instantiate(prefabObj, gameObject.transform.position, gameObject.transform.rotation);

            if (foundMat != null)
            {
                // might need to be modified for more complex player models
                body.transform.GetChild(0).GetComponent<MeshRenderer>().material = foundMat;
            }
        }
        else if (prefab)
        {
            // Spawn their copy of their body
            GameObject body = Instantiate(prefab, gameObject.transform.position, gameObject.transform.rotation);

            if (foundMat != null)
            {
                // might need to be modified for more complex player models
                body.transform.GetChild(0).GetComponent<MeshRenderer>().material = foundMat;
            }
        }
    }
}