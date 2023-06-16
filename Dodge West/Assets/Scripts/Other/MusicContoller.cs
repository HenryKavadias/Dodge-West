using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the music for the scene
public class MusicContoller : MonoBehaviour
{
    public GameObject objectMusic = null;

    [SerializeField]
    [Range(0f, 1f)]
    private float musicVolume = 0.5f;
    private AudioSource audioSource;
    
    // Gets music source, gets the audio source from it, and sets its volume
    void Start()
    {
        objectMusic = GameObject.FindWithTag("GameMusic");

        if (objectMusic)
        {
            audioSource = objectMusic.GetComponent<AudioSource>();

            audioSource.volume = musicVolume;
        }
    }

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
