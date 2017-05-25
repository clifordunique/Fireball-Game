/* Name: WaterDropletEnemy
 * Author: John Paul Depew
 * Description: The controller script for the water droplet enemy. This script sets its movement back and forth between waypoints,
 * and if the player is seen, the water droplet attacks.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropletEnemy : MonoBehaviour {

    public LayerMask mask;
    public float speed = 8;
    public float chaseSpeed = 12;
    public float waitTime = .2f;
    public Transform pathHolder;
    public float viewDistanceX = 8;
    public float viewDistanceY = 3;
    public float health = 10;
    // If an object damages the enemy, it should always have a PlayerWeapon script attached
    PlayerWeapon weapon;

    Transform player;
    Animator anim;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = gameObject.GetComponent<Animator>();

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
        Debug.Log("I am trying to chase the player");
        anim.SetFloat("Speed", chaseSpeed);
        //Vector2 dirToPlayer = (player.position - transform.position).normalized;
        while (transform.position.x != player.position.x)
        {
            Debug.Log("Player: " + player.position.x + "\nMe: " + transform.position.x);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), chaseSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Entered the trigger");
        if (col.gameObject.CompareTag("PlayerWeapon"))
        {
            if (col.gameObject.GetComponent<PlayerWeapon>() != null)
            {
                weapon = col.gameObject.GetComponent<PlayerWeapon>();
                DamageEnemy(weapon.damage);
                Destroy(col.gameObject);
            }
            else
            {
                Debug.Log("Invalid projectile. Either it shouldn't be labeled as PlayerWeapon, or it needs the PlayerWeapon script attached");
            }
        }
    }

    public void DamageEnemy(int _damage)
    {
        health -= _damage;
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
