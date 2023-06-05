using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class MuteManager : MonoBehaviour
{
    private bool pauseEnabled = true;
    
    private bool isMuted;
    private InputSystemUIInputModule iSUIIMMuteControl;
    private AudioSource audioSource;

    // Get mute state on start
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        iSUIIMMuteControl = GetComponent<InputSystemUIInputModule>();
        isMuted = PlayerPrefs.GetInt("MUTED") == 1;
        //AudioListener.pause = isMuted;
        if (isMuted)
        {
            Pause();
            AudioListener.volume = 0;
        }
    }

    private void Update()
    {
        // Note: need to figure out control method for muting audio

        if (iSUIIMMuteControl.cancel.action.triggered)
        {
            MutePressed();
        }
    }

    private void Pause()
    {
        if (pauseEnabled)
        {
            audioSource.Pause();
        }
    }

    private void UnPause()
    {
        if (pauseEnabled || !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }

    // Toggle mute state
    public void MutePressed()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("MUTED", isMuted ? 1 : 0);
        if (isMuted)
        {
            Pause();
            AudioListener.volume = 0; 
        }
        else
        {
            UnPause();
            AudioListener.volume = 1; 
        }
    }

    public void Mute()
    {
        isMuted = true;
        PlayerPrefs.GetInt("MUTED", 1);
        //AudioListener.pause = isMuted;
        if (isMuted)
        {
            Pause();
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
            UnPause();
            AudioListener.volume = 1;
        }
    }
}
