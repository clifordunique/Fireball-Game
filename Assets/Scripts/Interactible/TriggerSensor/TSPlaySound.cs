using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSPlaySound : TriggerSensor
{
    AudioManager audioManager;
    public string song = "song";

    private void Start()
    {
        audioManager = AudioManager.instance;
    }

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            audioManager.PlaySound(song);
            enabled = false;
            if (destroySelf)
            {
                Destroy(this);
            }
        }
    }
}
