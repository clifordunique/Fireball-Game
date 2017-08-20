/* Name: PlayerWeapon.cs
 * Author: John Paul Depew
 * Description: This script should be attached to every object that can hurt the enemy.
 * It damages the enemy on contact.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Transform explodeParticles;
    public int damage = 4;
    public LayerMask layerMask;
    public float rayLength = 2f;
    public Transform raycastPoint;

    Vector2 rotation;
    Vector3 hitNormal;
    float rotation2;
    Quaternion startRot;

    AudioManager audioManager;
    Enemy enemy;
    BoxCollider2D collider;

    void Start()
    {
        audioManager = AudioManager.instance;
        collider = GetComponent<BoxCollider2D>();
        rotation = FindObjectOfType<Attack>().GetDirection();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastPoint.position, rotation, rayLength, layerMask);
        Debug.DrawRay(raycastPoint.position, rotation * rayLength, Color.red);

        if (hit)
        {
            Quaternion.FromToRotation(Vector3.down, hitNormal);
            if (hit.collider.gameObject.tag == "Enemy")
            {
                HitEnemy(hit);
            }
            else if (hit.collider.gameObject.tag == "Obstacle")
            {
                Effect(hit.normal);
            }
        }
    }

    void HitEnemy(RaycastHit2D hit)
    {
        audioManager.PlaySound("Water Hiss Short");
        enemy = hit.collider.gameObject.GetComponent<Enemy>();
        enemy.DamageEnemy(damage, transform.position);
        Effect(hit.normal);
        Destroy(this.gameObject);
    }

    void Effect(Vector3 hitNormal)
    {
        Instantiate(explodeParticles, transform.position, Quaternion.FromToRotation(Vector3.left, hitNormal));
        Destroy(this.gameObject);
    }
}
