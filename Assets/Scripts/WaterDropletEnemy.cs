/* Name: WaterDropletEnemy
 * Author: John Paul Depew
 * Description: The controller script for the water droplet enemy. This script sets its movement back and forth between waypoints,
 * and if the player is seen, the water droplet attacks.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropletEnemy : MonoBehaviour , Enemy {

    public LayerMask mask;
    public float speed = 8;
    public float chaseSpeed = 12;
    public float waitTime = .2f;
    public Transform pathHolder;
    public float viewDistanceX = 8;
    public float viewDistanceY = 3;
    public float maxHealth = 10;

    float health;
    int damage = 10;

    // If an object damages the enemy, it should always have a PlayerWeapon script attached
    PlayerWeapon weapon;
    AudioManager audioManager;

    Transform player;
    Animator anim;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = gameObject.GetComponent<Animator>();
        health = maxHealth;
        audioManager = AudioManager.instance;

        Vector2[] waypoints = new Vector2[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        StartCoroutine(FollowPath(waypoints));
	}
	
	void Update () {
        if (CanSeePlayer())
        {
            StopAllCoroutines();
            StartCoroutine(ChasePlayer());
        }
    }

    /* Tests whether the enemy can see the player or not
     * @returns true if the enemy can see the player
     */
    bool CanSeePlayer()
    {
        if (Mathf.Abs(transform.position.x - player.position.x) < viewDistanceX && Mathf.Abs(transform.position.y - player.position.y) < viewDistanceY)
        {
            if ((Mathf.Sign(transform.localScale.x) > 0 && (player.position.x > transform.position.x)) || Mathf.Sign(transform.localScale.x) < 0 && (player.position.x < transform.position.x))
            {
                if (!Physics.Linecast(transform.position, player.position, mask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector2[] waypoints)
    {
        // Corretly placing enemy at initialization
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector2 targetWaypoint = waypoints[targetWaypointIndex];
        float dirX = Mathf.Sign(targetWaypoint.x - transform.position.x);
        transform.localScale = new Vector3(dirX*transform.localScale.x, transform.localScale.y, transform.localScale.z);

        while (true)
        {
            anim.SetFloat("Speed", speed);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetWaypoint.x, transform.position.y), speed * Time.deltaTime);
            if (transform.position.x == targetWaypoint.x)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                anim.SetFloat("Speed", 0);
                yield return new WaitForSeconds(waitTime);

                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);

                yield return new WaitForSeconds(waitTime);
                anim.SetFloat("Speed", speed);
            }
            yield return null;
        }
    }
    
    IEnumerator ChasePlayer()
    {
        //Debug.Log("I am trying to chase the player");
        anim.SetFloat("Speed", chaseSpeed);

        while (transform.position.x != player.position.x)
        {
            //Debug.Log("Player: " + player.position.x + "\nMe: " + transform.position.x);
            float dirToPlayerX = (player.position.x - transform.position.x);

            Debug.Log(dirToPlayerX);
            
            if(dirToPlayerX > 0)
            {
                transform.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                transform.GetComponent<SpriteRenderer>().flipX = true;
            }
            //transform.Translate(dirToPlayerX * Time.deltaTime * chaseSpeed);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), chaseSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /* NOT NEEDED -- I now have the object detecting what it collided with
     * Detects when a trigger has entered the enemy's collider.
     * If it has a "PlayerWeapon" script attached, it scales the enemy and calls DamageEnemy.
     *
    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("Entered the trigger");
        if (col.gameObject.GetComponent<PlayerWeapon>() != null)
        {
            weapon = col.gameObject.GetComponent<PlayerWeapon>();
            DamageEnemy(weapon.damage);
            Destroy(col.gameObject);
        }
    }*/

    /* Damages the enemy and destroys it if its health is less than zero
     */
    public void DamageEnemy(int _damage)
    {
        health -= _damage;
        transform.localScale *= (health + 6 / health) / maxHealth;  // Weird equation for scaling the enemy on hits - maybe make it better
        audioManager.PlaySound("Spat");

        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<Player>() != null)
        {
            Player player = col.gameObject.GetComponent<Player>();
            player.DamageFire((int)(damage * ((health + 6 / health) / maxHealth)));
        }
    }
}