using System;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    SFX,
    Music
}

[Serializable]
public class Sound
{
    public SoundType soundType;
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public List<Sound> sounds;
    private Dictionary<string, AudioClip> soundClips = new Dictionary<string, AudioClip>();
    private Dictionary<SoundType, AudioSource> audioSources = new Dictionary<SoundType, AudioSource>();

    private float musicVolume = 1.0f;
    private float sfxVolume = 1.0f;

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

        // Initialize audio sources
        foreach (SoundType soundType in Enum.GetValues(typeof(SoundType)))
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            audioSources[soundType] = source;
        }

        // Populate sound clips dictionary for quick access
        foreach (var sound in sounds)
        {
            soundClips[sound.name] = sound.clip;
        }
    }

    // This needs to be placed on the audio source (object that triggers the sound)
    public void PlaySound(string soundName, SoundType soundType)
    {
        if (soundClips.TryGetValue(soundName, out AudioClip clip))
        {
            AudioSource source = audioSources[soundType];
            source.clip = clip;
            source.volume = (soundType == SoundType.Music) ? musicVolume : sfxVolume;
            source.Play();
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        audioSources[SoundType.Music].volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioSources[SoundType.SFX].volume = sfxVolume;
    }
}
