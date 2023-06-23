using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// NOTE: if any other Canvas exist in the scene, make sure the
// cross fade image canvas has the HIGHEST value for "sort order"

// Handles the transitions between scene
public class SceneTransition : MonoBehaviour
{
    public float transitionDuration = 1.0f; // Duration of transition (should allow for length of the animation)
    public Animator transition;             // Holds reference to animation

    // Transtion UI variables
    public GameObject transitionCanvas;
    public int canvasSortOrder = 1;
    
    // Next scene name (MUST be accurate)
    [SerializeField]
    private string nextScene = "Start-Scene";

    // Don't Destroy On Load by pass
    private bool bypassDNDOL = false;

    [SerializeField]
    private float timeLeft = 5f;
    private bool timerOn = false;

    public void SetNextScene(string Scene, bool ignoreDNDOL = false)
    {
        nextScene = Scene;
        bypassDNDOL = ignoreDNDOL;
    }

    // Enables timer when script is enabled
    private void OnEnable()
    {
        timerOn = true;
    }

    private void OnDisable()
    {
        timerOn = false;
    }

    // When timer is finished, start scene transition
    void Update()
    {
        if (timerOn)
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

                LoadNextScene();
            }
        }
    }

    // Destroy Player configuration manager from don't destroy on load if the bypass variable is false
    void CancelDontNotDestroyOnLoad()
    {
        GameObject configManager = 
            GameObject.FindGameObjectWithTag("ConfigurationManager");
        
        if (configManager != null)
        {
            Destroy(configManager);
        }
    }

    // Loadeds the scene it is given
    public void LoadNextScene(string scene = null, bool ignoreDNDOL = false)
    {
        Time.timeScale = 1.0f; // unpauses game
        bypassDNDOL = ignoreDNDOL;

        // Set canvas sort order
        transitionCanvas.GetComponent<Canvas>().sortingOrder = canvasSortOrder;

        if (scene == null)
        {
            StartCoroutine(LoadScene(nextScene));
        }
        else
        {
            StartCoroutine(LoadScene(scene));
        }
    }

    // Loading bar variables
    public GameObject loadingScreen;
    public Slider slider;
    public TextMeshProUGUI progressText;

    IEnumerator LoadScene(string scene)
    {
        // Play animation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionDuration);

        // Unpause audio
        GameObject gm = GameObject.FindWithTag("GameController");
        if (gm && gm.GetComponent<MusicContoller>() && gm.GetComponent<MusicContoller>().isPaused)
        {
            gm.GetComponent<MusicContoller>().PauseMusic(false);
        }


        // Load Scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        // Destroy player configuration manager if triggered to
        if (!bypassDNDOL)
        {
            CancelDontNotDestroyOnLoad();
        }

        // Activate loading bar
        loadingScreen.SetActive(true);
        loadingScreen.GetComponent<Canvas>().sortingOrder = canvasSortOrder + 1;

        // Update loading bar until next scene is loaded
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            
            slider.value = progress;
            // limited to 1 decimal point
            progressText.text = (progress * 100f).ToString("F1") + "%";

            yield return null;
        }
    }

    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        //timerTxt.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
}
