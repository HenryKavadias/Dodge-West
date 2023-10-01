using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeout : MonoBehaviour
{
    public bool inputTimeout = false;
    public float timeoutTrigger = 60f;
    public string startSceneName = string.Empty;

    private float currentTime = 0;

    public static Timeout TimeoutInstance { get; private set; }

    private void Awake()
    {
        ResetTimer();

        // Create instance of LevelDataContainer, store it in static variable,
        // assign object to don't destroy on load instance (destory self if another already exists)
        if (TimeoutInstance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a second " +
                "instance of a singleton class (game data container).");
            DestroySelf(); // removes the duplicate 
        }
        else
        {

            TimeoutInstance = this;
            DontDestroyOnLoad(TimeoutInstance);
        }
    }

    private void Update()
    {
        if (inputTimeout)
        {
            if (Input.anyKey)
            {
                ResetTimer();
            }
            
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                GameObject gameObject = GameObject.FindGameObjectWithTag("SceneTransitioner");
                if (gameObject != null)
                {
                    if (GameData.GameDataInstance != null)
                    {
                        GameData.GameDataInstance.DestroySelf();
                    }

                    ResetTimer();
                    if (startSceneName != string.Empty)
                    {
                        // This gets rid of player configuration manager if it exists
                        gameObject.GetComponent<SceneTransition>().LoadNextScene(startSceneName);
                    }
                }
            }
        }
    }

    public void ResetTimer()
    {
        currentTime = timeoutTrigger;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
