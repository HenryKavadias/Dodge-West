using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public bool instantTransition = false;
    
    [SerializeField]
    private string nextScene = "Start-Scene";

    [SerializeField]
    private float timeLeft = 5f;
    private bool timerOn = false;

    

    // Start is called before the first frame update
    void Start()
    {
        //timerOn = true;
    }

    private void OnEnable()
    {
        timerOn = true;
    }

    private void OnDisable()
    {
        timerOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn && !instantTransition)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Transition Now!");
                timeLeft = 0;
                timerOn = false;

                CancelDontNotDestroyOnLoad();
                LoadNextScene();
            }
        }
        else if (instantTransition)
        {
            timerOn = false;
            Time.timeScale = 1.0f; // unpauses game
            CancelDontNotDestroyOnLoad();
            LoadNextScene();
        }
    }

    void CancelDontNotDestroyOnLoad()
    {
        GameObject configManager = 
            GameObject.FindGameObjectWithTag("ConfigurationManager");
        
        if (configManager != null)
        {
            Destroy(configManager);
        }
    }

    void LoadNextScene()
    {

        SceneManager.LoadScene(nextScene);
    }    

    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        //timerTxt.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
}
