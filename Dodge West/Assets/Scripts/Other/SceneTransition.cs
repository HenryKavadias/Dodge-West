using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// NOTE: if any other Canvas exist in the scene, make sure the
// cross fade image canvas has the HIGHEST "sort order"
public class SceneTransition : MonoBehaviour
{
    public float transitionDuration = 1.0f;
    public Animator transition;

    public GameObject transitionCanvas;
    public int canvasSortOrder = 1;
    
    
    [SerializeField]
    private string nextScene = "Start-Scene";

    private bool bypassDNDOL = false;

    [SerializeField]
    private float timeLeft = 5f;
    private bool timerOn = false;

    public void SetNextScene(string Scene, bool ignoreDNDOL = false)
    {
        nextScene = Scene;
        bypassDNDOL = ignoreDNDOL;
    }

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

                //CancelDontNotDestroyOnLoad();
                LoadNextScene();
            }
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

    IEnumerator LoadScene(string scene)
    {
        // Play animation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionDuration);

        // Load Scene
        SceneManager.LoadScene(scene);

        if (!bypassDNDOL)
        {
            CancelDontNotDestroyOnLoad();
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
