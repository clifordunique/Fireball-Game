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
    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);
        if(col.gameObject.CompareTag("Rock"))
        {
            DamageEnemy(1000, col.transform.position);
        }
    }
}
