// Author: Kermit Mitchell III, Adapted From Brackeys (https://www.youtube.com/watch?v=6OT43pvUyfY)
// Start Date: 03/24/2020 8:35 PM | Last Edited: 03/24/2020 10:00 PM
// This script manages all game audio and acts as central authority on Audio

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // a singleton of the AudioManager
    public Dictionary<AudioName, Sound> sounds; // all sounds in the game
    public AudioSource backgroundMusic; // the audioSource linked to background music

    private void Awake()
    {
        // Singleton Check
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);


        // Initalize the sounds dictionary and add each sound
        sounds = new Dictionary<AudioName, Sound>();
        sounds.Add(AudioName.BGM1, new Sound(AudioName.BGM1, Resources.Load<AudioClip>("_Audio/Music/bgm1 - banjos"), null, 0.1f));
        sounds.Add(AudioName.BGM2, new Sound(AudioName.BGM2, Resources.Load<AudioClip>("_Audio/Music/bgm2 - pipa"), null, 0.1f));
        sounds.Add(AudioName.BGM3, new Sound(AudioName.BGM3, Resources.Load<AudioClip>("_Audio/Music/bgm3 - comedy"), null, 0.1f));
        sounds.Add(AudioName.BGM4, new Sound(AudioName.BGM4, Resources.Load<AudioClip>("_Audio/Music/bgm4 - tranquil"), null, 0.1f));
        sounds.Add(AudioName.BGM5, new Sound(AudioName.BGM5, Resources.Load<AudioClip>("_Audio/Music/bgm5 - 8-bit hiphop"), null, 0.1f));

        // Setup all the sounds
        foreach (Sound s in sounds.Values)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    private void Start()
    {
        // Grab the reference to the Background Audio Source
        instance.backgroundMusic = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
    }

    // Plays any audio file based on AudioName
    public void Play(AudioName name)
    {
        Sound s = sounds[name];
        if(s == null)
        {
            Debug.LogError("Time: " + Time.time + "Error! No audio found for AudioName: " + name);
            return;
        }

        // If AudioName is a Background Music, then stop other background musics, and play this one
        if((int)name <= 5 && (int)name >= 1)
        {
            instance.backgroundMusic.Stop();
            instance.backgroundMusic = s.source;
            instance.backgroundMusic.Play();
        }
        else
        {
            s.source.Play();
        }
        
    }
}


// Class for Organizing Sounds
[System.Serializable] // Makes this appear in Unity Editor
public class Sound
{
    public AudioName name;
    public AudioClip clip;
    /*[HideInInspector]*/ public AudioSource source;

    [Range(0.0f, 1.0f)] public float volume; // how loud this AudioClip is
    [Range(0.1f, 3.0f)] public float pitch; // the pitch and speed of the audio clip

    public Sound()
    {
        name = AudioName.NULL;
        clip = null;
        volume = 1.0f;
        pitch = 1.0f;
        source = null;
    }

    public Sound(AudioName name, AudioClip clip, AudioSource source, float volume = 1.0f, float pitch = 1.0f)
    {
        this.name = name;
        this.clip = clip;
        this.source = source;
        this.volume = volume;
        this.pitch = pitch;
    }
}

// A valid list of all audio names to direct to
public enum AudioName
{
    NULL = 0,
    BGM1 = 1, // background music 1 - banjo
    BGM2 = 2, // background music 2 - pipa
    BGM3 = 3, // background music 3 - comedy
    BGM4 = 4, // background music 4 - tranquil
    BGM5 = 5 // background music 5 - 8-bit hiphop
}


