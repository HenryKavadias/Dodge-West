using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool twoPlayer = false;

    public GameObject playerObject;

    public GameObject[] spawnPosition;

    private Quaternion spawnRot = Quaternion.identity;

    private void Start()
    {
        if (playerObject)
        {
            if (twoPlayer)
            {
                if (spawnPosition[0])
                {
                    GameObject p1 = Instantiate(playerObject, 
                        spawnPosition[0].GetComponent<Transform>().position, spawnRot);
                    p1.GetComponent<PlayerID>().ChangePlayerNumber(1);
                }

                if (spawnPosition[1])
                {
                    GameObject p2 = Instantiate(playerObject, 
                        spawnPosition[1].GetComponent<Transform>().position, spawnRot);
                    p2.GetComponent<PlayerID>().ChangePlayerNumber(2);
                }
            }
            else
            {
                if (spawnPosition[0])
                {
                    GameObject p1 = Instantiate(playerObject, 
                        spawnPosition[0].GetComponent<Transform>().position, spawnRot);
                    p1.GetComponent<PlayerID>().ChangePlayerNumber(0);
                }
            }
        }
    }

}
