using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeFire : MonoBehaviour {

    public float healAmount = 1;

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.GetComponent<Player>() != null)
        {
            Player player = col.GetComponent<Player>();
            player.HealFire(healAmount);
        }
    }
}
