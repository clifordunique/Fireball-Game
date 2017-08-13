using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoHorn : MonoBehaviour {

    Player player;
    public Camera cam;
    public float directionX = -1;
    public Transform rayOriginPos;
    //public delegate void OnRhinoHitPlayer();
    //public event OnRhinoHitPlayer rhinoHitPlayerEvent;
    public int horizontalRayCount = 7;
    public float rayLength = 1f;
    public LayerMask collisionMask;

    float horizontalRaySpacing = 1f;

    void Start()
    {
        //FindObjectOfType<Controller2D>().rhinoHitPlayerEvent += OnRhinoHitPlayer;
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        ConstantHorizontalCollisions(rayOriginPos.position);
    }

    void OnRhinoHitPlayer()
    {
        Debug.Log("one step farther!!!");
        player.DamagePlayer(1000);
        //FindObjectOfType<Controller2D>().rhinoHitPlayerEvent -= OnRhinoHitPlayer;
    }

    /*Detects Collisions with the player from the rhino's horn
     * 
     */
    void ConstantHorizontalCollisions(Vector2 rayOriginPos)
    {
        Vector2 rayOrigin = rayOriginPos;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            rayOrigin += Vector2.down * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            //Debug.Log("rayOrigin: " + rayOrigin);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                if(hit.collider.tag == "Player")
                {
                    StartCoroutine(ZoomCamera());
                    OnRhinoHitPlayer();
                }
                else if(hit.collider.tag == "Rock")
                {
                    hit.collider.gameObject.GetComponent<ISmashable>().DestroyObject();
                }
            }
        }
    }

    IEnumerator ZoomCamera()
    {
        while (cam.orthographicSize > 10)
        {
            cam.orthographicSize -= 0.5f;
            yield return null;
        }
    }
}
