using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour, ISmashable {

    public Transform explodePrefab;
    AudioManager audioManager;

    void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
        }
    }

    public void DestroyObject()
    {
        audioManager.PlaySound("Rock Smash");
        Instantiate(explodePrefab, new Vector2(transform.position.x, transform.position.y + 1), transform.rotation);
        Destroy(gameObject);
    }
}
