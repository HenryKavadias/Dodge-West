using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Sound
{
    //public SoundType soundType;
    public string name;
    public AudioClip clip;
}

// Holds and managers all the sounds and music tracks in the game
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public List<Sound> sounds;
    public List<Sound> musicList;

    private bool loopMusic = true;

    [Range(0f, 1f)]
    public float musicVolume = 1.0f;
    [Range(0f, 1f)]
    public float sfxVolume = 1.0f;

    private AudioSource audioSource;

    // Updates the volume for the music audio source
    public void UpdateMusicVolume(float Music)
    {
        audioSource.volume = Music;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("[Singleton] Trying to instantiate a second " +
                "instance of a singleton class (level data container).");
            Destroy(gameObject); // removes the duplicate 
            return;
        }

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    //private void Start()
    //{
    //    audioSource = GetComponent<AudioSource>();
    //}

    public List<Sound> GetSounds()
    {
        return sounds;
    }

    private float startTime = 0f;
    private float stopTime = 0f;
    public string currentTrack { get; private set; } = string.Empty;

    // Starts a music track at a specific start point with a delay and
    // end point in the track. Also controls if the track loops
    public void BeginMusicTrack(
        string name, 
        float start = 0.0f, 
        float stop = 0.0f, 
        float delay = 0.0f, 
        bool loop = true)
    {
        audioSource.Stop();

        // check if music is accessable
        bool pass = false;
        int listCounter = 0;

        foreach (Sound music in musicList)
        {
            if (music.name == name)
            {
                pass = true;
                break;
            }
            listCounter++;
        }

        if (!pass)
        { return; }

        currentTrack = name;

        stopTime = stop;

        AudioClip musicClip = musicList[listCounter].clip;

        audioSource.volume = musicVolume;

        audioSource.clip = musicClip;

        if (start > 0f)
        {
            audioSource.time = start - delay;
        }
        startTime = start;

        loopMusic = loop;

        audioSource.Play();
        audioSource.loop = true;
    }

    public void EndMusicTrack()
    {
        audioSource.Stop();
    }

    private void Update()
    {
        if (startTime > 0f && audioSource.time > stopTime)
        {
            audioSource.Stop();

            if (loopMusic)
            {
                audioSource.time = startTime;
                audioSource.Play();
            }
        }
    }
}
