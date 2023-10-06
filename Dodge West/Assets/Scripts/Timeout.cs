using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

// Resets the game back to the "Start Scene" after a period of no player input or if a specific input is triggered
public class Timeout : MonoBehaviour
{
    public bool inputTimeout = false;
    public bool buttonReset = false;
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

    // Triggers a scene transition and removes all Do not destory on load objects that don't need to persist
    private void ResetGame()
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
                ResetGame();
            }
        }

        if (buttonReset && GetComponent<InputSystemUIInputModule>() != null 
            && GetComponent<InputSystemUIInputModule>().cancel.action.triggered)
        {
            ResetGame();
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
