using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script allows game objects to follow the camera from a certain offset
 */
public class FollowCamera : MonoBehaviour
{

    public int offsetX = 1;
    public int offsetY = 10;
    public Transform target;

    void Update()
    {
        transform.position = new Vector2(target.transform.position.x + offsetX, target.transform.position.y + offsetY);
    }
}
