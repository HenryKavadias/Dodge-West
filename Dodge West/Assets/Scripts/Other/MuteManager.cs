using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

// Allows the player to mute the audio for the game
public class MuteManager : MonoBehaviour
{
    // The mute state is saved, even when the game is turned off
    
    // Tracks pause state
    private bool pauseEnabled = true;
    
    private bool isMuted;   // Tracks mute state
    private InputSystemUIInputModule iSUIIMMuteControl; // Reference to component that holds the controls for the mute button
    private AudioSource audioSource;    // Reference to audio sources

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
        // Note: need to figure out control method for muting audio.
        // Currently just uses a seperate UI controller to manage mute state

        if (iSUIIMMuteControl.cancel.action.triggered)
        {
            MutePressed();
        }
    }

    // Pause audio
    private void Pause()
    {
        if (pauseEnabled)
        {
            audioSource.Pause();
        }
    }

    // Unpause Audio
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

    // Mute audio
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

    // Unmute audio
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
