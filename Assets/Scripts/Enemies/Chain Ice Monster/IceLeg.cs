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

    void Effect(Vector2 position)
    {
        Instantiate(spriteMask, position, transform.rotation, transform);
    }
}
