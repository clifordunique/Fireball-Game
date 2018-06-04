using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceLeg : Enemy
{
    public Transform metalBrace;
    public SpriteMask spriteMask;

    public override void DamageEnemy(int _damage, Vector2 position)
    {
        health -= _damage;
        if (health <= 0)
        {
            metalBrace.parent = transform.parent;
            Rigidbody2D metalRb2D = metalBrace.GetComponent<Rigidbody2D>();

            metalBrace.GetComponent<BoxCollider2D>().enabled = true;
            metalRb2D.bodyType = RigidbodyType2D.Dynamic;
            metalRb2D.mass = 50;
            metalRb2D.gravityScale = 5;
            
            Destroy(this.gameObject);
        }
        Effect(position);
    }

    /// <summary>
    /// Creates effect for damaging enemy
    /// </summary>
    /// <param name="hitPos">The position of the hit on the enemy</param>
    void Effect(Vector2 hitPos)
    {
        float dirToHit = Mathf.Sign(hitPos.x - transform.position.x);
        float offset = dirToHit > 0 ? -0.2f : 0.2f;
        Vector3 angle = dirToHit > 0 ? -spriteMask.transform.rotation.eulerAngles : spriteMask.transform.rotation.eulerAngles;
        SpriteMask newSpriteMask = Instantiate(spriteMask, new Vector2(hitPos.x + offset, hitPos.y), Quaternion.Euler(angle));
        newSpriteMask.transform.localScale = new Vector3(newSpriteMask.transform.localScale.x * (-dirToHit), newSpriteMask.transform.localScale.y, newSpriteMask.transform.localScale.y);
        newSpriteMask.transform.SetParent(transform);
    }
}
