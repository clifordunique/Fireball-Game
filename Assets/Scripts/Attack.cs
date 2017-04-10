﻿/* Author: John Paul Depew
 * Allows whatever object this is attached to to shoot
 * using the mouse
 */

using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class Attack : MonoBehaviour
{

    public Transform test;
    public Transform firePoint;
    public float speed = 10f;
    Stopwatch sw;

    void Start()
    {
        firePoint = transform.FindChild("FirePoint");
        sw = new Stopwatch();
    }

    void Update()
    {
        sw.Start();
        UnityEngine.Debug.Log(sw.ElapsedMilliseconds);
        if (sw.ElapsedMilliseconds > 500)
        {
            if (firePoint == null)
            {
                UnityEngine.Debug.LogError("No firepoint? WHAT?!");
            }
            if (Input.GetMouseButtonDown(0))
            {
                sw.Reset();
                Shoot();
            }
            
        }
        
    }

    void Shoot()
    {
        Vector2 targetPoint = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 heading = targetPoint - (Vector2)firePoint.transform.position;
        Vector2 direction = heading.normalized;
        float angle = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
        UnityEngine.Debug.Log(angle);
        UnityEngine.Debug.Log(firePoint.position);

        Instantiate(test, firePoint.position, Quaternion.FromToRotation(Vector3.up, direction));
        UnityEngine.Debug.Log("Speed: " + direction * speed);
    }
}


