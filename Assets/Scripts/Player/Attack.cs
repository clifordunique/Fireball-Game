/* Author: John Paul Depew
 * Allows whatever object this is attached to to shoot
 * using the mouse
 */

using UnityEngine;
using System.Diagnostics;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Player))]
public class Attack : MonoBehaviour
{
    public PlayerWeapon fireball;
    public Transform firePoint;
    public float speed = 10f;
    Stopwatch sw;
    Vector2 direction;

    AudioManager audioManager;
    PlayerStats stats;

    void Start()
    {
        stats = PlayerStats.instance;
        firePoint = transform.Find("FirePoint");
        sw = new Stopwatch();
        audioManager = AudioManager.instance;
    }

    void Update()
    {
        if (stats.isFire())
        {
            sw.Start();
            if (sw.ElapsedMilliseconds > 500)
            {
                if (firePoint == null)
                {
                    UnityEngine.Debug.LogError("No firepoint? WHAT?!");
                }
                if (Input.GetMouseButtonDown(0))
                {
                    sw.Reset();
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        Shoot();
                    }
                }
            }
        }
    }

    void Shoot()
    {
        Vector2 targetPoint = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 heading = targetPoint - (Vector2)firePoint.transform.position;
        direction = heading.normalized;
        float angle = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
        audioManager.PlaySound("Fireball");
        //UnityEngine.Debug.Log(angle);

        Instantiate(fireball, firePoint.position, Quaternion.FromToRotation(Vector3.up, direction));
        //UnityEngine.Debug.Log("Speed: " + direction * speed);
    }

    public Vector2 GetDirection()
    {
        return direction;
    }
}


