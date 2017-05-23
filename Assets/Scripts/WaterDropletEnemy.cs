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
    public float waitTime = .2f;
    public Transform pathHolder;
    public float viewDistanceX = 8;
    public float viewDistanceY = 3;

    Transform player;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Vector2[] waypoints = new Vector2[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        StartCoroutine(FollowPath(waypoints));
	}
	
	void Update () {
        bool temp = CanSeePlayer();
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
                    Debug.Log("I can see the player. Prepare for obliteration.");
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
            transform.position = Vector2.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position.x == targetWaypoint.x)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);

                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);

                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }
    }
}
