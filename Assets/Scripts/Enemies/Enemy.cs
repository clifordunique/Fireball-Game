using UnityEngine;

public class Enemy: MonoBehaviour {

    public float maxHealth = 10;
    public float health;
    public float seePlayerDistanceX = 8;
    public float seePlayerDistanceY = 3;
    public bool cameraFollow = false;

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