using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the music for the scene
public class MusicContoller : MonoBehaviour
{
    private GameObject objectMusic = null;

    public string track = string.Empty;
    public float startOfTrack = 0f;
    public float endOfTrack = 0f;
    public float initialDelay = 0f;
    public bool loopTrack = true;

    [SerializeField]
    [Range(0f, 1f)]
    private float defaultMusicVolume = 0.5f;   // default volume
    private AudioSource audioSource;
    private AudioManager audioManager;
    
    // Gets music source, gets the audio source from it, and sets its volume
    void Start()
    {
        objectMusic = GameObject.FindWithTag("GameAudio");

        if (objectMusic)
        {
            audioSource = objectMusic.GetComponent<AudioSource>();

            audioManager = objectMusic.GetComponent<AudioManager>();
            
            if (audioManager != null ) 
            {
                audioSource.volume = audioManager.musicVolume;

                if (track != string.Empty)
                {
                    audioManager.BeginMusicTrack(track, startOfTrack, endOfTrack, initialDelay, loopTrack);
                }
            }
            else
            {
                audioSource.volume = defaultMusicVolume;
            }
        }
    }

    public void StopTrack()
    {
        if (audioManager != null)
        {
            audioManager.EndMusicTrack();
        }
    }

    // Below is used for pause controls

    // Tracks pause state of music
    public bool isPaused { get; private set; }

    // Toggles pause state of music
    public void PauseMusic(bool pause = true)
    {
        if (!audioSource) { return; }

        if (pause && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
        isPaused = pause;
    }
}
