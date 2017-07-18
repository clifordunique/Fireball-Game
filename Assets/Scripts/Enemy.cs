using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemy {

    void DamageEnemy(int _damage);
    void Effect(Vector2 position);
}