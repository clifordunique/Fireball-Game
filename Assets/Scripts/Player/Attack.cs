/* Author: John Paul Depew
 * Allows Player to shoot using the mouse.
 * Idea: This script probably controls too much. Maybe the play input script should take care of mouse input and send it to here or something.
 * Another script should take care of the animation, like maybe the player script.
 */

using UnityEngine;
using System.Diagnostics;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Player))]
public class Attack : MonoBehaviour
{
    public PlayerWeapon fireball;
    public PlayerWeapon fireBurst;
    public Transform firePoint;
    public float speed = 10f;
    public Animator headAnim;
    public float mouthSpeed = 0.1f;

    Stopwatch sw1;
    Vector2 direction;

    AudioManager audioManager;
    PlayerStats stats;

    void Start()
    {
        stats = PlayerStats.instance;
        firePoint = transform.Find("FirePoint");
        sw1 = new Stopwatch();
        audioManager = AudioManager.instance;
    }

    void Update()
    {
        // First check and see if the state is paused. If it is, return.
        if (GameMaster.gm.CurState == Utilities.State.PAUSED) return;

        if (stats.IsFire())
        {
            sw1.Start();
            if (sw1.ElapsedMilliseconds > 100)
            {
                if (firePoint == null)
                {
                    UnityEngine.Debug.LogError("No firepoint? WHAT?!");
                }
                if (Input.GetMouseButtonDown(0))
                {
                    sw1.Reset();
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (stats.Shoot)
                        {
                            Shoot();
                        }
                        else
                        {
                            Burst();
                        }
                    }
                }
            }
        }
    }

    void Burst()
    {
        Instantiate(fireBurst, firePoint.position - 0.4f * Vector3.up, transform.rotation, this.transform);
        Effect();
    }

    void Shoot()
    {
        Vector2 targetPoint = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 heading = targetPoint - (Vector2)firePoint.transform.position;
        direction = heading.normalized;
        Effect();

        Instantiate(fireball, firePoint.position, Quaternion.FromToRotation(Vector3.up, direction));
    }

    /// <summary>
    /// This function starts the stopwatch and plays the fireball sound
    /// </summary>
    void Effect()
    {
        StopAllCoroutines();
        StartCoroutine(OpenMouth());
        audioManager.PlaySound("Fireball");
    }

    /// <summary>
    /// Changes the BlowingFire parameter for the golem head to 1, which opens the mouth.
    /// </summary>
    /// <returns>null</returns>
    IEnumerator OpenMouth()
    {
        float blowingFire = headAnim.GetFloat("BlowingFire");

        while (blowingFire < 1)
        {
            // It is going to 1.1 instead of 1 because otherwise it'll keep lerping into infinity
            blowingFire = Mathf.Lerp(blowingFire, 1.1f, mouthSpeed);
            headAnim.SetFloat("BlowingFire", blowingFire);
            yield return null;
        }
        StartCoroutine(CloseMouth());
    }

    /// <summary>
    /// Changes the BlowingFire parameter for the golem head to 0, which closes the mouth.
    /// </summary>
    /// <returns>null</returns>
    IEnumerator CloseMouth()
    {
        float blowingFire = headAnim.GetFloat("BlowingFire");

        while (blowingFire > 0)
        {
            // It is going to -0.1 instead of 0 because otherwise it'll keep lerping into infinity
            blowingFire = Mathf.Lerp(blowingFire, -0.1f, mouthSpeed);
            headAnim.SetFloat("BlowingFire", blowingFire);
            yield return null;
        }
    }

    public Vector2 GetDirection()
    {
        return direction;
    }
}


