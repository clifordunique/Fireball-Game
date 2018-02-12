using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script allows game objects to follow the camera from a certain offset
 */
public class FollowCamera : MonoBehaviour {

    public int offsetX = 1;
    public int offsetY = 10;
    public Transform[] targets;

	void Start () {
		
	}
	
	void Update () {
		for (int i = 0; i < targets.Length; i++)
        {
            targets[i].transform.position = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
        }
	}
}
