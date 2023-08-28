using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeLeft;
    public bool timerOn = false;
    public bool onlySeconds = true;

    public string endMessage = "Let it rain!";
    public TextMeshProUGUI timerText;
    public RectTransform timerBase;
    public float timerWidth = 200f;
    public float messageWidth = 600f;

    public void SetAndStart(float time = 60f, bool start = false)
    {
        timeLeft = time;
        timerOn = start;

        timerBase.sizeDelta = new Vector2(timerWidth, timerBase.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {
            if(timeLeft > 0f)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
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
