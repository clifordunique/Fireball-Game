using UnityEngine;

public class Enemy: MonoBehaviour {

    public float maxHealth = 10;
    public float health;
    public float seePlayerDistanceX = 8;
    public float seePlayerDistanceY = 3;
    public int damageToPlayerFire = 10;
    public int damageToPlayerHealth = 10;
    public bool cameraFollow = false;
    protected DamagePlayerData damagePlayerData;

    public virtual void Start()
    {
        health = maxHealth;
        damagePlayerData = gameObject.AddComponent<DamagePlayerData>();
        damagePlayerData.damageToPlayerFireHealth = damageToPlayerFire;
        damagePlayerData.damageToPlayerHealth = damageToPlayerHealth;
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