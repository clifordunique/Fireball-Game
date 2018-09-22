using UnityEngine;

public class CameraShake : MonoBehaviour
{
    CameraFollow cam;

    float shakeAmount = 0;
    float offsetX = 0;
    float offsetY = 0;
    float newOffsetY = 0;
    float newOffsetX = 0;
    float timer = 11;

    void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main.GetComponent<CameraFollow>();
        }
    }

    /// <summary>
    /// Shakes the camera
    /// </summary>
    /// <param name="amt">The amount to displace/shake the camera</param>
    /// <param name="length">The amount of time to shake the camera</param>
    public void Shake(float amt, float length)
    {
        shakeAmount = amt;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }

    void DoShake()
    {
        if (shakeAmount > 0)
        {
            if (timer > 10)
            {
                timer = 0;
                newOffsetX = Random.value * shakeAmount * 2 - shakeAmount;
                newOffsetY = Random.value * shakeAmount * 2 - shakeAmount;
            }

            offsetX = Mathf.Lerp(offsetX, newOffsetX, 0.2f);
            offsetY = Mathf.Lerp(offsetY, newOffsetY, 0.2f);

            Vector3 camPos = cam.transform.position;
            cam.UpdateShake(offsetX, offsetY);
            timer++;
        }
    }

    void StopShake()
    {
        CancelInvoke("DoShake");
        cam.UpdateShake(0, 0);
        timer = 11;
    }
}
