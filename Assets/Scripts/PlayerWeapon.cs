/* Name: PlayerWeapon.cs
 * Author: John Paul Depew
 * Description: This script should be attached to every object that can hurt the enemy.
 * It damages the enemy on contact.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {

    Enemy enemy;
    public int damage = 4;
    AudioManager audioManager;

    void Start()
    {
        audioManager = AudioManager.instance;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Enemy>() != null)
        {
            audioManager.PlaySound("Water Hiss Short");
            enemy = col.gameObject.GetComponent<Enemy>();
            enemy.DamageEnemy(damage, transform.position);
            Destroy(this.gameObject);
        }
    }
}
