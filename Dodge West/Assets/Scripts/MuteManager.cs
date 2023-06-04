using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteManager : MonoBehaviour
{
    private bool isMuted;

    // Get mute state on start
    void Start()
    {
        isMuted = PlayerPrefs.GetInt("MUTED") == 1;
        //AudioListener.pause = isMuted;
        if (isMuted)
        {
            AudioListener.volume = 0;
        }
    }

    private void Update()
    {
        // Note: need to figure out control method for muting audio
        
        //if ()
        //{
        //    MutePressed();
        //}
    }

    // Toggle mute state
    public void MutePressed()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("MUTED", isMuted ? 1 : 0);
        if (isMuted)
        { AudioListener.volume = 0; }
        else
        { AudioListener.volume = 1; }
    }

    public void Mute()
    {
        isMuted = true;
        PlayerPrefs.GetInt("MUTED", 1);
        //AudioListener.pause = isMuted;
        if (isMuted)
        {
            AudioListener.volume = 0;
        }
    }

    public void Unmute()
    {
        isMuted = false;
        PlayerPrefs.GetInt("MUTED", 0);
        //AudioListener.pause = isMuted;
        if (isMuted)
        {
            AudioListener.volume = 0;
        }
    }
}
