using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// Timer script mostly used for UI purposes
public class Timer : MonoBehaviour
{
    public float startGameTime = 3f;
    public string startMessage = "GO!";
    private bool beginGameDelay = true;

    public float timeLeft;
    public bool timerOn = false;
    public bool onlySeconds = true;

    public string endMessage = "Let it rain!";
    public TextMeshProUGUI timerText;
    public RectTransform timerBase;
    public float timerWidth = 200f;
    public float messageWidth = 600f;

    private RainingObjects rainingObjects = null;

    private void Awake()
    {
        rainingObjects = GetComponent<RainingObjects>();
    }

    // Start the countdown with an intial delay
    public void TriggerStartDelay()
    {
        
        if (beginGameDelay)
        {
            SetAndStart(startGameTime, true);
        }
        else
        {
            BeginRainCountDown();
        }
    }

    // Starts the rain objects count down
    public void BeginRainCountDown()
    {
        if (rainingObjects)
        {
            rainingObjects.BeginCountDown();
        }
    }

    // Set the timer and start it.
    public void SetAndStart(float time = 60f, bool start = false)
    {
        timeLeft = time;
        timerOn = start;

        timerBase.sizeDelta = new Vector2(timerWidth, timerBase.sizeDelta.y);
    }

    // Timer tracking run in update
    void Update()
    {
        if (timerOn)
        {
            if(timeLeft > 0f)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
            }
            else if (beginGameDelay)
            {
                beginGameDelay = false;

                timerText.text = startMessage;
                timerOn = false;

                // Ensure player controls are re-enabled
                gameObject.GetComponent<GameController>().TogglePlayerControls(true);

                Invoke(nameof(BeginRainCountDown), 1.5f);
            }
            else
            {
                Debug.Log("Time's up.");
                timeLeft = 0f;
                timerOn = false;

                timerText.text = endMessage;
                timerBase.sizeDelta = new Vector2(
                    messageWidth, timerBase.sizeDelta.y);
                this.enabled = false;
            }
        }
    }

    // Updates the UI of the timer
    void UpdateTimer(float time)
    {
        time += 1;

        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        //timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (onlySeconds)
        {
            seconds = Mathf.FloorToInt(time);
            timerText.text = seconds.ToString();
        }
        else if (minutes >= 1)
        {
            timerText.text =
                string.Format(
                    "{0:00}:{1:00}",
                    minutes, seconds);
        }
        else
        {
            timerText.text = seconds.ToString();
        }
    }
}
