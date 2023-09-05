using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for keeping constant score between players
public class GameData : MonoBehaviour
{
    // player ID and score
    private Dictionary<int, int> data = new Dictionary<int, int>();
    
    public void SetPlayerList(List<GameObject> players)
    {
        foreach (GameObject player in players)
        {
            data.Add(player.GetComponent<PlayerID>().GetID(), 0);
        }
    }

    public int GetPlayerCount() { return data.Count; }

    public Dictionary<int, int> GetPlayers()
    {
        return data;
    }

    public void AddScore(int winner, int score)
    {
        //foreach(var player in data) 
        //{ 
        //    Debug.Log(player.Key + ": " +  player.Value);
        //}
        
        data[winner] += score;
    }

    // Might not be needed
    public static GameData GameDataInstance { get; private set; }

    private void Awake()
    {
        // Create instance of LevelDataContainer, store it in static variable,
        // assign object to don't destroy on load instance (destory self if another already exists)
        if (GameDataInstance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a second " +
                "instance of a singleton class (game data container).");
            DestroySelf(); // removes the duplicate 
        }
        else
        {

            GameDataInstance = this;
            DontDestroyOnLoad(GameDataInstance);
        }
    }

    public void SetGameDataInstance()
    {
        GameDataInstance = this;
    }    

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
