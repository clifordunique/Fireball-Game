﻿/* Name: AudioManager.cs
 * Author: Brackeys
 * Description: Contains a Sound and AudioManager class. The sound class has settings for volume, pitch, randomness, and looping.
 * AudioManager contains an array of Sound objects and functions for playing and stopping the sound.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1;

    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    public bool loop = false;

    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.Play();
    }

    public void Play(float _volumePercent)
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f)) * _volumePercent;
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.Play();
    }

    public void SetVolume(float _volume)
    {
        source.volume = _volume;
    }

    public void Pause()
    {
        source.Pause();
    }

    public void UnPause()
    {
        source.UnPause();
    }

    public bool isPlaying()
    {
        return source.isPlaying;
    }

    public void Stop()
    {
        source.Stop();
    }
}

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [SerializeField]
    Sound[] sounds;
    private float volumePercent = 1;

    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }

    /* Plays the sound
     * @param _name - the sound to be played
     */
    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play(volumePercent);
                return;
            }
        }

        // no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list, " + _name);
    }

    public void PlaySound(string _name, float _volumePercent)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play(_volumePercent);
                return;
            }
        }

        // no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list, " + _name);
    }

    /* Stops the sound
     * @param _name - the sound to be stopped
     */
    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Stop();
                return;
            }
        }

        // no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list, " + _name);
    }

    public void Fade(string sound1, string sound2)
    {
        Sound tempSound1 = new Sound();
        Sound tempSound2 = new Sound();
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == sound1)
            {
                tempSound1 = sounds[i];
            }
        }
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == sound2)
            {
                tempSound2 = sounds[i];
            }
        }
        StartCoroutine(FadePitch(tempSound1,tempSound2));
    }

    IEnumerator FadePitch(Sound tempSound1, Sound tempSound2)
    {
        // getting original volume levels
        float tempVolume1 = tempSound1.volume;
        float tempVolume2 = tempSound2.volume;

        // playing tempsound1 at 0 volume
        tempSound1.Play(0);
        for (float i = 0; i < tempVolume1; i += 0.001f)
        {
            tempSound1.SetVolume(i);
            tempSound1.volume = i;
            tempSound2.SetVolume(tempVolume2 - i);
            tempSound2.volume = tempVolume2 - i;
            yield return null;
        }
        tempSound2.Stop();
    }

    /* Pass in the string to check if it is playing
     */
    public bool isPlaying(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                return sounds[i].isPlaying();
            }
        }
        Debug.LogWarning("AudioManager: Sound not found in list, " + _name);
        return false;
    }
}