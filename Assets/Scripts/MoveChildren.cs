using System.Collections ;
using System.Collections.Generic;
using UnityEngine;

public class MoveChildren : MonoBehaviour {

    public Transform[] water;
    public Transform[] pathHolder;
    public float moveSpeed = 1;

    AudioManager audioManager;


    void Start () {
        audioManager = AudioManager.instance;
        Vector2[] waypoints = null;
        for (int j = 0; j < pathHolder.Length; j++)
        {
            waypoints = new Vector2[pathHolder[j].childCount];
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = pathHolder[j].GetChild(i).position;
            }
            StartCoroutine(FollowPath(waypoints, water[j]));
        }

    }
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator FollowPath(Vector2[] waypoints, Transform waterToMove)
    {
        // Corretly placing enemy at initialization
        waterToMove.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector2 targetWaypoint = waypoints[targetWaypointIndex];
        //float dirX = Mathf.Sign(targetWaypoint.x - transform.position.x);
        while (true)
        {
            waterToMove.position = Vector2.MoveTowards(waterToMove.position, targetWaypoint, moveSpeed * Time.deltaTime);
            if (waterToMove.position.x == targetWaypoint.x && waterToMove.position.y == targetWaypoint.y)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
            }
            yield return null;
        }
    }
}
