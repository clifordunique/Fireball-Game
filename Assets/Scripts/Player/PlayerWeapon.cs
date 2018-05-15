/* Name: PlayerWeapon.cs
 * Author: John Paul Depew
 * Description: This script should be attached to every object that can hurt the enemy.
 * It damages the enemy on contact.
 */

using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Transform explodeParticles;
    public int damage = 4;
    public LayerMask layerMask;
    public float rayLength = 2f;
    public Transform raycastPoint;

    Vector2 rotation;
    float rotation2;
    Quaternion startRot;

    Enemy enemy;

    void Start()
    {
        rotation = FindObjectOfType<Attack>().GetDirection();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastPoint.position, rotation, rayLength, layerMask);
        Debug.DrawRay(raycastPoint.position, rotation * rayLength, Color.red);

        if (hit)
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                HitEnemy(hit);
            }
            else
            {
                Effect(hit);
            }
        }
    }

    void HitEnemy(RaycastHit2D hit)
    {
        enemy = hit.collider.gameObject.GetComponent<Enemy>();
        enemy.DamageEnemy(damage, transform.position);
        if(hit.collider.GetComponent<WaterDropletEnemy>() == null)
        {
            Effect(hit);
        }
        Destroy(this.gameObject);
    }

    void Effect(RaycastHit2D hit)
    {
        Instantiate(explodeParticles, hit.point + hit.normal * 0.7f, Quaternion.FromToRotation(Vector3.left, hit.normal));
        Destroy(this.gameObject);
    }
}
