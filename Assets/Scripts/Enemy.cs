using UnityEngine;

public class Enemy: MonoBehaviour {

    public float maxHealth = 10;
    protected float health;

    public virtual void Start()
    {
        health = maxHealth;
    }

    public virtual void DamageEnemy(int _damage, Vector2 position)
    {
        health -= _damage;
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}