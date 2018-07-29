using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSPlaySound : TriggerSensor
{
    AudioManager audioManager;
    public Utilities.Song Song;

    private void Start()
    {
        audioManager = AudioManager.instance;
    }

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameMaster.gm.SetBackgroundSong(Song);
            enabled = false;
            if (destroySelf)
            {
                Destroy(this);
            }
        }
    }
}
