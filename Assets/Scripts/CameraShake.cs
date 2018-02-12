using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public Camera mainCam;
    public CameraFollow cam;

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

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;
            cam.UpdateShake(offsetX, offsetY);
            camPos.x += offsetX;
            camPos.y += offsetY;

            mainCam.transform.position = camPos;
        }
    }

    void StopShake()
    {
        CancelInvoke("DoShake");
        cam.UpdateShake(0, 0);
        mainCam.transform.localPosition = originalPos;
    }
}
