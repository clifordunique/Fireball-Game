using UnityEngine;

public class CameraShake : MonoBehaviour
{
    CameraFollow cam;

    float shakeAmount = 0;

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
            Vector3 camPos = cam.transform.position;

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;
            cam.UpdateShake(offsetX, offsetY);
        }
    }

    void StopShake()
    {
        CancelInvoke("DoShake");
        cam.UpdateShake(0, 0);
    }
}
