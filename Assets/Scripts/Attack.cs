using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour
{

    public Transform test;
    public Transform firePoint;
    public float speed = 10f;

    void Update()
    {
        firePoint = transform.FindChild("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("No firepoint? WHAT?!");
        }
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector2 targetPoint = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 heading = targetPoint - (Vector2)firePoint.transform.position;
        Vector2 direction = heading.normalized;
        float angle = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
        Debug.Log(angle);
        Debug.Log(firePoint.position);

        Instantiate(test, firePoint.position, Quaternion.FromToRotation(Vector3.up, direction));
        Debug.Log("Speed: " + direction * speed);
    }
}

// Unneccesary way of calculating the angle:
// 


