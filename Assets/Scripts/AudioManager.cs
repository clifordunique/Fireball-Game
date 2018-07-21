/* Name: AudioManager.cs
 * Author: Brackeys and John Paul Depew
 * Description: Contains a Sound and AudioManager class. The sound class has settings for volume, pitch, randomness, and looping.
 * AudioManager contains an array of Sound objects and functions for playing and stopping the sound.
 */

using System.Collections;
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

    public float GetVolume()
    {
        return source.volume;
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

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    [SerializeField]
    Sound[] sounds;

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
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }

    public float GetVolume(string _name)
    {
        Sound tempSound = FindSound(_name);
        return tempSound.GetVolume();
    }

    /// <summary>
    /// Play a sound
    /// </summary>
    /// <param name="_name">The name of the sound to be played</param>
    public void PlaySound(string _name)
    {
        Sound tempSound = FindSound(_name);
        if (tempSound != null)
            tempSound.Play();
        else
            Debug.LogWarning("Trying to play a nonexistent sound");
    }

    /// <summary>
    /// Play a sound at a certain volume percentage
    /// </summary>
    /// <param name="_name">The name of the sound to be played</param>
    /// <param name="_volumePercent">The volume percentage for the sound</param>
    public void PlaySound(string _name, float _volumePercent)
    {
        Sound tempSound = FindSound(_name);
        tempSound.Play(_volumePercent);
    }

    /// <summary>
    /// This stops the given sound
    /// </summary>
    /// <param name="_name">The sound to be stopped</param>
    public void StopSound(string _name)
    {
        Sound tempSound = FindSound(_name);
        tempSound.Stop();
    }

    /// <summary>
    /// Fades sound1 in and sound2 out at the same speed
    /// </summary>
    /// <param name="sound1">The sound to fade in</param>
    /// <param name="sound2">The sound to fade out</param>
    public void FadeBetweenTwoSounds(string sound1, string sound2)
    {
        Sound tempSound1 = FindSound(sound1);
        Sound tempSound2 = FindSound(sound2);
        StartCoroutine(FadeVolume(tempSound1, tempSound2));
    }

    /// <summary>
    /// Fades sound to a level given by volumePercentage
    /// </summary>
    /// <param name="sound">Sound to fade</param>
    /// <param name="volumePercentage">Volume percentage for the sound</param>
    /// <param name="fadeSpeed">The speed at which the sound fades out</param>
    public void FadeSound(string sound, float volumePercentage, float fadeSpeed)
    {
        Sound tempSound = FindSound(sound);
        StartCoroutine(FadeVolume(tempSound, volumePercentage, fadeSpeed));
    }

    /// <summary>
    /// This function fades one sound in and the other out concurrently
    /// </summary>
    /// <param name="tempSound1">The sound that will fade in</param>
    /// <param name="tempSound2">The sound that will fade out</param>
    /// <returns>null</returns>
    IEnumerator FadeVolume(Sound tempSound1, Sound tempSound2)
    {
        // getting original volume levels
        // Values tend to get set to something like 1.2E-508239238470293841230489, so we had to check for small values instead of zero
        float tempVolume1 = tempSound1.volume < 0.01f ? 1 : tempSound1.volume;
        float tempVolume2 = tempSound2.volume < 0.01f ? 1 : tempSound2.volume;

        float iOld = -1;

        // playing tempsound1 at 0 volume
        tempSound1.Play(0);
        for (float i = 0; i < tempVolume1; i += 0.005f)
        {
            if (iOld == i)
            {
                tempSound1.SetVolume(1);
                tempSound1.volume = 1;

                tempSound2.SetVolume(0);
                tempSound2.volume = 0;

                break;
            }
            // Yes, apparantly it is necessary to change both the source volume and the actual volume...
            tempSound1.SetVolume(i);
            tempSound1.volume = i;
            // calculation to decrease the volume of tempSound2 proportionally to tempSound1 since they might be at different volumes
            tempSound2.SetVolume(tempVolume2 - i * tempVolume2 / tempVolume1);
            tempSound2.volume = tempVolume2 - i * tempVolume2 / tempVolume1;

            iOld = i;
            yield return null;
        }
        tempSound2.Stop();
    }

    /// <summary>
    /// Fades a sound to a certain volume percentage by a certain speed between 0 and 1
    /// </summary>
    /// <param name="sound">The sound to fade</param>
    /// <param name="volumePercentage">The percentage between 0 and 1 to fade the sound to</param>
    /// <param name="fadeSpeed">The amount the sound increases by each frame</param>
    /// <returns></returns>
    IEnumerator FadeVolume(Sound sound, float volumePercentage, float fadeSpeed)
    {
        if (volumePercentage > sound.volume)
        {
            for (float i = sound.volume; i < volumePercentage; i += fadeSpeed)
            {
                sound.SetVolume(i);
                sound.volume = i;
                yield return null;
            }
        }
        else // volumePercentage < sound.volume
        {
            for (float i = sound.volume; i > volumePercentage; i -= fadeSpeed)
            {
                sound.SetVolume(i);
                sound.volume = i;
                yield return null;
            }
        }
    }

    /// <summary>
    /// This returns true or false depending on whether or not the given string is playing.
    /// </summary>
    /// <param name="_name">The sound which's playing status must be checked</param>
    /// <returns></returns>
    public bool isPlaying(string _name)
    {
        Sound tempSound = FindSound(_name);
        if (tempSound != null)
            return tempSound.isPlaying();
        else
            return false;
    }

    /// <summary>
    /// Checks the sounds array for a sound name
    /// </summary>
    /// <param name="_name">The sound to be searched for</param>
    /// <returns></returns>
    private Sound FindSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                return sounds[i];
            }
        }
        Debug.LogWarning("AudioManager: Sound not found in list, " + _name);
        return null;
    }
}