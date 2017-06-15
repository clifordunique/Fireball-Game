using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    AudioManager audioManager;
    public string[] forestBackgroundArray;
    int backgroundSoundIndex;

    void Start () {
        audioManager = AudioManager.instance;

        if (audioManager == null)
        {
            Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
        }
        GetRandomIndex();
        PlayBackgroundMusic();
    }

    void LateUpdate()
    {
        if(!audioManager.isPlaying(forestBackgroundArray[backgroundSoundIndex]))
        {
            GetRandomIndex();
            PlayBackgroundMusic();
        }
    }

    void GetRandomIndex()
    {
        backgroundSoundIndex = Random.Range(0, forestBackgroundArray.Length);
    }

    void PlayBackgroundMusic()
    {
        audioManager.PlaySound(forestBackgroundArray[backgroundSoundIndex]);
    }
}
