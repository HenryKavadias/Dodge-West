using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayers : MonoBehaviour
{
    public bool killOnStart = true;
    
    // Start is called before the first frame update
    void Start()
    {
        if (killOnStart)
        {
            KillAllPlayers();
        }
    }

    public void KillAllPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<Damageable>().Damage(player.GetComponent<Health>().Initial + 100);
        }
    }
}
