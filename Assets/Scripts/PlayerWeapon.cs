/* Name: PlayerWeapon.cs
 * Author: John Paul Depew
 * Description: This script should be attached to every object that can hurt the enemy.
 * It damages the enemy on contact.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {

    public Transform explodeParticles;
    public int damage = 4;
    public LayerMask layerMask;
    public float rayLength = 2f;

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
        Vector2 bottomRaycastOrigin = new Vector2(collider.bounds.min.x, collider.bounds.min.y);
        Vector2 topRaycastOrigin = new Vector2(collider.bounds.min.x, collider.bounds.max.y);

        RaycastHit2D hitBottom = Physics2D.Raycast(bottomRaycastOrigin, rotation, rayLength, layerMask);
        RaycastHit2D hitTop = Physics2D.Raycast(topRaycastOrigin, rotation, rayLength, layerMask);
        Debug.DrawRay(bottomRaycastOrigin, rotation * rayLength, Color.red);
        Debug.DrawRay(topRaycastOrigin, rotation * rayLength, Color.red);
        if(hitBottom)
        {
            hitNormal = hitBottom.normal;
            Quaternion.FromToRotation(Vector3.down, hitNormal);
            Effect(hitNormal);
            //hitRotation = hitBottom.normal;
            //Debug.Log("hitRotation.z " + hitRotation);
            //rotation2 = Vector2.Angle(Vector2.up, hitBottom.normal);

            //startRot = Quaternion.LookRotation(hitBottom.normal);

            //float slopeAngle = Vector2.Angle(hitBottom.normal, Vector2.up);
        }
    }

    // TODO: Change to raycasts to detect collision
    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.GetComponent<Enemy>() != null)
        {
            audioManager.PlaySound("Water Hiss Short");
            enemy = col.gameObject.GetComponent<Enemy>();
            enemy.DamageEnemy(damage, transform.position);
            Destroy(this.gameObject);
        }
    }

    void Effect(Vector3 hitNormal)
    {
        Instantiate(explodeParticles, transform.position, Quaternion.FromToRotation(Vector3.left, hitNormal));
        Destroy(this.gameObject);
    }

    public void SetDirection(Vector2 _direction)
    {
        rotation = _direction;
    }
}
