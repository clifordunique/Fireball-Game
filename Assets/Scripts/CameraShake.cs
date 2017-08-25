﻿using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public Camera mainCam;

    Vector2 originalPos;
    float shakeAmount = 0;

    void Awake()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    void Update()
    {
        Vector3 camPos = mainCam.transform.position;
        Debug.Log(camPos);
    }

    public void Shake(float amt, float length)
    {
        shakeAmount = amt;
        originalPos = mainCam.transform.position;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }

    void DoShake()
    {
        if (shakeAmount > 0)
        {
            Vector3 camPos = mainCam.transform.position;
            Debug.Log(camPos);

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;
            camPos.x += offsetX;
            camPos.y += offsetY;

            Debug.Log("offset: " + camPos);
            mainCam.transform.position = camPos;
        }
    }

    void StopShake()
    {
        CancelInvoke("DoShake");
        mainCam.transform.localPosition = originalPos;
    }
}
