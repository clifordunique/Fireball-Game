using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGuy : PathFollowingEnemies {

    public ObjectExplode rockExplosion;

    protected override void Effect(Vector2 position)
    {
        float dirToDamage = Mathf.Sign(position.x - transform.position.x);

        Vector2 xMaxMin = dirToDamage > 0 ? new Vector2(1, 0) : new Vector2(0, -1);
        rockExplosion = Instantiate(rockExplosion,transform.position,transform.rotation);
        rockExplosion.maxThrust = 1000;
        rockExplosion.minThrust = 300;
        rockExplosion.xMaxMin = xMaxMin;

        audioManager.PlaySound("rockSmash");
    }
}
