/* Name: WaterDropletEnemy
 * Author: John Paul Depew
 * Description: The controller script for the water droplet enemy. This script sets its movement back and forth between waypoints,
 * and if the player is seen, the water droplet attacks.
 */

using UnityEngine;

public class WaterDropletEnemy : PathFollowingEnemies
{
    public Transform waterSplat;

    public override void Start()
    {
        base.Start();
    }

    protected override void Effect(Vector2 hitPosition)
    {
        //TODO: make it so the splash has a forward velocity if the enemy has a forward velocity so that the splash is visible
        audioManager.PlaySound("Spat");
        audioManager.PlaySound("Water Hiss Short");

        // Facing you
        if ((transform.localScale.x > 0 && hitPosition.x < transform.position.x) || (transform.localScale.x < 0 && hitPosition.x > transform.position.x))
        {
            // Player is to the left
            if (hitPosition.x < transform.position.x)
            {
                if (anim.GetFloat("Speed") == 0)
                {
                    Transform tempWaterSplat = Instantiate(waterSplat, new Vector2(hitPosition.x + 2, hitPosition.y), Quaternion.Euler(Vector2.right));
                    tempWaterSplat.localScale = new Vector2(-tempWaterSplat.localScale.x, tempWaterSplat.localScale.y);
                }
                else
                {
                    Transform tempWaterSplat = Instantiate(waterSplat, hitPosition, Quaternion.Euler(Vector2.right));
                    tempWaterSplat.localScale = new Vector2(-tempWaterSplat.localScale.x, tempWaterSplat.localScale.y);
                }
            }
            else // Player is to the right
            {
                if (anim.GetFloat("Speed") == 0)
                {
                    Instantiate(waterSplat, new Vector2(hitPosition.x - 2, hitPosition.y), Quaternion.Euler(Vector2.right));
                }
                else
                {
                    Instantiate(waterSplat, hitPosition, Quaternion.Euler(Vector2.right));
                }
            }
        }
        else // not facing the player
        {
            // Player is to the left
            if (hitPosition.x < transform.position.x)
            {
                Instantiate(waterSplat, hitPosition, Quaternion.Euler(Vector2.right));
            }
            else // Player is to the right
            {
                Transform tempWaterSplat = Instantiate(waterSplat, hitPosition, Quaternion.Euler(Vector2.right));
                tempWaterSplat.localScale = new Vector2(-tempWaterSplat.localScale.x, tempWaterSplat.localScale.y);
            }
        }
    }
}