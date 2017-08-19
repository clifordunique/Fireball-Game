using UnityEngine;

public interface Enemy {

    void DamageEnemy(int _damage, Vector2 position);
    void Effect(Vector2 position);
}