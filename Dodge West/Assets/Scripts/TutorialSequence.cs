using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class TutorialSequence : MonoBehaviour
{
    private bool enableTutorial = false;

    public GameObject rootOfPanel = null;
    public InputSystemUIInputModule uiControls = null;
    public GameController gameController = null;
    public Pause pauseScript = null;
    // Panels should be arranged in intended order
    public List<GameObject> tutorialPanels = new List<GameObject>();
    private GameObject currentPanel = null;
    private int currentPanelNumber = 0;

    private float ignoreInputTime = 1.0f;
    private bool inputEnabled = false;

    void Start()
    {
        if (uiControls == null || 
            rootOfPanel == null || 
            pauseScript == null ||
            tutorialPanels.Count <= 0 ||
            gameController == null)
        {
            enableTutorial = false;

            rootOfPanel.SetActive(false);
        }
        else
        {
            enableTutorial = true;

            rootOfPanel.SetActive(true);

            // limit player controls
            gameController.TogglePlayerControls(false);

            pauseScript.inTutorial = true;

            currentPanel = Instantiate(
                tutorialPanels[currentPanelNumber], 
                rootOfPanel.transform);

            //Debug.Log("Start: " + currentPanelNumber);
        }

    }

    void Update()
    {
        if (enableTutorial)
        {
            if (Time.time > ignoreInputTime)
            {
                inputEnabled = true;
            }

            if (inputEnabled)
            {
                if (uiControls.submit.action.triggered)
                {
                    //Debug.Log("Next: " + currentPanelNumber);
                    if ((currentPanelNumber + 1) < tutorialPanels.Count)
                    {
                        Destroy(currentPanel);
                        currentPanel = null;

                        currentPanelNumber++;
                        currentPanel = Instantiate(
                            tutorialPanels[currentPanelNumber],
                            rootOfPanel.transform);
                    }
                    else
                    {
                        // conclude tutorial, re-enable pause script
                        Destroy(currentPanel);
                        currentPanel = null;

                        currentPanelNumber = 0;

                        rootOfPanel.SetActive(false);

                        //pauseScript.enabled = true;
                        pauseScript.inTutorial = false;

                        // Enable player controls
                        gameController.TogglePlayerControls(true);

                        // Disables this script
                        enableTutorial = false;
                    }

                    inputEnabled = false;
                }
                else if (uiControls.cancel.action.triggered)
                {
                    //Debug.Log("Back: " + currentPanelNumber);
                    if ((currentPanelNumber - 1) >= 0)
                    {
                        Destroy(currentPanel);
                        currentPanel = null;

                        currentPanelNumber--;

                        currentPanel = Instantiate(
                            tutorialPanels[currentPanelNumber],
                            rootOfPanel.transform);
                    }

                    inputEnabled = false;
                }
            }
        }
        else
        {
            // Disables this script
            enabled = false;
        }
    }
}
