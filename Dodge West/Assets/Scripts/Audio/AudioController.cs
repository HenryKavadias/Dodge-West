using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public float pitchRange = 0.1f;

    [Header("Must be accurate")]
    public List<string> soundNames = new List<string>();
    public List<string> triggerSoundNames = new List<string>();

    //public bool playOnStart = false;

    private List<Sound> objTriggerSounds = new List<Sound>();
    private List<Sound> objSounds = new List<Sound>();

    private AudioSource audioSource;

    [Range(0f, 1f)]
    private float volume = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        List<Sound> soundList = null;
        soundList = GameObject.FindGameObjectWithTag("GameAudio").
            gameObject.GetComponent<AudioManager>().GetSounds();

        if (soundList != null)
        {
            volume = GameObject.FindGameObjectWithTag("GameAudio").
            gameObject.GetComponent<AudioManager>().sfxVolume;
            foreach (Sound sound in soundList)
            {
                foreach (string wanted in soundNames)
                {
                    if (sound.name == wanted)
                    {
                        objSounds.Add(sound);
                    }
                }

                foreach (string wanted in triggerSoundNames)
                {
                    if (sound.name == wanted)
                    {
                        objTriggerSounds.Add(sound);
                    }
                }
            }
        }

        if (gameObject.GetComponent<AudioSource>() == null)
        {
            gameObject.AddComponent<AudioSource>();
        }

        audioSource = gameObject.GetComponent<AudioSource>();

        //if (playOnStart)
        //{
        //    RandomSfxOnStart();
        //}
    }

    public void AssignStartSounds(List<string> sounds)
    {
        triggerSoundNames = sounds;

        // Refresh
        Start();

        RandomSfxOnAssign();
    }

    public void RandomSfxOnAssign()
    {
        // Randomly pick a sound from the current list
        int randomIndex = Random.Range(0, objTriggerSounds.Count);
        string soundName = triggerSoundNames[randomIndex];

        // check if sound is accessable to object
        bool pass = false;
        int listCounter = 0;

        foreach (string sound in triggerSoundNames)
        {
            if (sound == soundName)
            {
                pass = true;
                break;
            }
            listCounter++;
        }

        if (!pass)
        { return; }

        audioSource.Stop();

        AudioClip soundClip = objTriggerSounds[listCounter].clip;

        float lowPitch = 1f - pitchRange;
        float highPitch = 1f + pitchRange;

        audioSource.volume = volume;

        audioSource.pitch = Random.Range(lowPitch, highPitch);

        audioSource.PlayOneShot(soundClip);
    }

    public void TriggerAudio(string soundName)
    {
        // check if sound is accessable to object
        bool pass = false;
        int listCounter = 0;

        foreach (string sound in soundNames)
        {
            if (sound == soundName)
            {
                pass = true;
                break;
            }
            listCounter++;
        }

        if (!pass)
        { return; }

        audioSource.Stop();

        AudioClip soundClip = objSounds[listCounter].clip;

        float lowPitch = 1f - pitchRange;
        float highPitch = 1f + pitchRange;

        audioSource.volume = volume;

        audioSource.pitch = Random.Range(lowPitch, highPitch);

        audioSource.PlayOneShot(soundClip);
    }

    // unfinished
    public void RandomizeSfx()
    {
        // Randomly pick a sound from the current list
        int randomIndex = Random.Range(0, soundNames.Count);
        string soundName = soundNames[randomIndex];

        //Debug.Log(soundName);

        // check if sound is accessable to object
        bool pass = false;
        int listCounter = 0;

        foreach (string sound in soundNames)
        {
            if (sound == soundName)
            {
                pass = true;
                break;
            }
            listCounter++;
        }

        if (!pass)
        { return; }

        audioSource.Stop();
        
        AudioClip soundClip = objSounds[listCounter].clip;

        float lowPitch = 1f - pitchRange;
        float highPitch = 1f + pitchRange;

        audioSource.volume = volume;

        audioSource.pitch = Random.Range(lowPitch, highPitch);

        audioSource.PlayOneShot(soundClip);
    }
}
