using System.Collections ;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    public Transform[] water;
    public Transform[] pathHolder;
    public float moveSpeed = 10;
    public float smoothDistance = 4;
    public int damage = 10;

    AudioManager audioManager;
    PlayerStats stats;

    bool wasFire = false;

    void Start () {
        audioManager = AudioManager.instance;
        stats = PlayerStats.instance;

        //Vector2[] waypoints = null;
        //for (int j = 0; j < pathHolder.Length; j++)
        //{
        //    waypoints = new Vector2[pathHolder[j].childCount];
        //    for (int i = 0; i < waypoints.Length; i++)
        //    {
        //        waypoints[i] = pathHolder[j].GetChild(i).position;
        //    }
        //    StartCoroutine(FollowPath(waypoints, water[j]));
        //}

    }

    /* Detects a collision
     * decreases the players firehealth (probably to zero)
     */
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Player>() != null)
        {
            Player player = col.gameObject.GetComponent<Player>();
            player.DamageFire(damage);
            if (!stats.IsFire() && wasFire)
            {
                audioManager.StopSound("Water Hiss Long");
                audioManager.PlaySound("Water Hiss End");
                wasFire = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Player>() != null)
        {
            if (stats.IsFire())
            {
                wasFire = true;
                audioManager.PlaySound("Water Hiss Long");
            }
        }
        if(col.gameObject.GetComponent<FallInWaterableObject>() != null)
        {
            FallInWaterableObject waterable = col.gameObject.GetComponent<FallInWaterableObject>();
            waterable.SetIsInWater(true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Player>() != null)
        {
            if (stats.IsFire() || wasFire)
            {
                audioManager.StopSound("Water Hiss Long");
                audioManager.PlaySound("Water Hiss End");
                wasFire = false;
            }
        }
        if (col.gameObject.GetComponent<FallInWaterableObject>() != null)
        {
            FallInWaterableObject waterable = col.gameObject.GetComponent<FallInWaterableObject>();
            waterable.SetIsInWater(false);
        }
    }

    IEnumerator FollowPath(Vector2[] waypoints, Transform waterToMove)
    {
        // Corretly placing enemy at initialization
        waterToMove.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector2 targetWaypoint = waypoints[targetWaypointIndex];

        while (true)
        {
            float distTargetX = Mathf.Abs(targetWaypoint.x - waterToMove.position.x);
            float distTargetY = Mathf.Abs(targetWaypoint.y - waterToMove.position.y);
            float distOldX = Mathf.Abs(waypoints[(targetWaypointIndex + 1) % waypoints.Length].x - waterToMove.position.x);
            float distOldY = Mathf.Abs(waypoints[(targetWaypointIndex + 1) % waypoints.Length].y - waterToMove.position.y);
            float targetMagnitude = new Vector2(distTargetX, distTargetY).magnitude;
            float oldMagnitude = new Vector2(distOldX, distOldY).magnitude;

            if (waterToMove.position.x == targetWaypoint.x && waterToMove.position.y == targetWaypoint.y)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
            }

            // smoothing - REALLY BAD AND NOT EXACTLY WORKING
            if(targetMagnitude < smoothDistance)
            {
                
                waterToMove.position = Vector2.MoveTowards(waterToMove.position, targetWaypoint, (targetMagnitude + .1f) / smoothDistance * moveSpeed * Time.deltaTime);
            }
            else if (oldMagnitude < smoothDistance)
            {
                waterToMove.position = Vector2.MoveTowards(waterToMove.position, targetWaypoint, (oldMagnitude + .1f) / smoothDistance * moveSpeed * Time.deltaTime);
            }
            else
            {
                waterToMove.position = Vector2.MoveTowards(waterToMove.position, targetWaypoint, moveSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }
}
