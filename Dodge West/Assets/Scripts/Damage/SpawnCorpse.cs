using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnCorpse : Spawner
{
    //public GameObject playerModel;
    public MeshRenderer playerRender;

    public bool usingCharacterModels = false;

    public Transform bodyRotation = null;

    public GameObject playerHat = null;
    public Transform playerHatSpawn = null;

    private void Start()
    {
        if (usingCharacterModels)
        {
            switch (gameObject.GetComponent<PlayerInputHandler>().modelSelected)
            {
                case CharacterModel.dog:
                    playerRender = gameObject.GetComponent<PlayerInputHandler>().dogMesh;
                    break;
                case CharacterModel.cat:
                    playerRender = gameObject.GetComponent<PlayerInputHandler>().catMesh;
                    break;
                default:
                    break;
            }
        }
    }

    protected override void CreateObject(GameObject prefab = null)
    {
        // Find and copy the players material onto the copy of their body
        Object[] matResources = Resources.LoadAll("Materials", typeof(Material));

        Material foundMat = null;

        //Material playerMat = playerModel.GetComponent<MeshRenderer>().material;

        Material playerMat = playerRender.material;

        foreach (Object obj in matResources)
        {
            Material current = (Material)obj;

            if (playerMat.name.ToString().Contains(current.name))
            {
                foundMat = new Material(current);
                break;
            }
        }

        GameObject body = null;

        if (bodyRotation != null)
        {
            body = Instantiate(prefab, transform.position,
            Quaternion.Euler(bodyRotation.eulerAngles.x,
            bodyRotation.eulerAngles.y,
            bodyRotation.eulerAngles.z));
        }
        else
        {
            body = Instantiate(prefab, transform.position,
            Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y,
            transform.eulerAngles.z));
        }

        var rb = GetComponent<Rigidbody>();

        var objRb = body.gameObject.GetComponent<Rigidbody>();

        objRb.velocity = rb.velocity;
        objRb.angularVelocity = rb.angularVelocity;

        if (playerHat != null && playerHatSpawn != null)
        {
            GameObject hat = null;

            if (bodyRotation != null)
            {
                hat = Instantiate(playerHat, playerHatSpawn.position,
                    Quaternion.Euler(bodyRotation.eulerAngles.x,
                    bodyRotation.eulerAngles.y,
                    bodyRotation.eulerAngles.z));
            }
            else
            {
                hat = Instantiate(playerHat, playerHatSpawn.position,
                    Quaternion.Euler(transform.eulerAngles.x,
                    transform.eulerAngles.y,
                    transform.eulerAngles.z));
            }
            
            var hatRb = hat.gameObject.GetComponent<Rigidbody>();
            hatRb.velocity = rb.velocity;
            hatRb.angularVelocity = rb.angularVelocity;
        }

        if (foundMat != null)
        {
            // NOTE: toggle off the inactive model
            if (usingCharacterModels)
            {
                //Debug.Log(gameObject.GetComponent<PlayerInputHandler>().modelSelected);
                switch (gameObject.GetComponent<PlayerInputHandler>().modelSelected)
                {
                    case CharacterModel.dog:
                        //Debug.Log("Dog");
                        body.GetComponent<CorpseData>().dogRender.material = foundMat;
                        body.GetComponent<CorpseData>().dogModel.SetActive(true);
                        body.GetComponent<CorpseData>().catModel.SetActive(false);
                        break;
                    case CharacterModel.cat:
                        //Debug.Log("Cat");
                        body.GetComponent<CorpseData>().catRender.material = foundMat;
                        body.GetComponent<CorpseData>().dogModel.SetActive(false);
                        body.GetComponent<CorpseData>().catModel.SetActive(true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                body.transform.GetChild(0).GetComponent<MeshRenderer>().material = foundMat;
            }
        }
    }
}