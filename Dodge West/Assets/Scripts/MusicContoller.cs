using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicContoller : MonoBehaviour
{
    public GameObject objectMusic;

    [SerializeField]
    [Range(0f, 1f)]
    private float musicVolume = 0.5f;
    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        objectMusic = GameObject.FindWithTag("GameMusic");
        audioSource = objectMusic.GetComponent<AudioSource>();

        audioSource.volume = musicVolume;

    }

    public bool isPaused { get; private set; }

    public void PauseMusic(bool pause = true)
    {
        if (pause)
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
