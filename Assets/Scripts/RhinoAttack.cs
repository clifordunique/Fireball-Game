using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoAttack : MonoBehaviour {


    public Camera cam;
    public float directionX = -1;
    public Transform rayOriginPos;
    //public delegate void OnRhinoHitPlayer();
    //public event OnRhinoHitPlayer rhinoHitPlayerEvent;
    public int horizontalRayCount = 7;
    public float rayLength = 1f;
    public LayerMask collisionMask;
    public float horizontalRaySpacing = 1f;

    Player player;
    CameraShake camShake;

    void Start()
    {
        //FindObjectOfType<Controller2D>().rhinoHitPlayerEvent += OnRhinoHitPlayer;
        player = FindObjectOfType<Player>();
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if (camShake == null)
        {
            Debug.LogError("No CameraShake found on the GameMaster.");
        }
    }

    void Update()
    {
        ConstantHorizontalCollisions(rayOriginPos.position);
    }

    void OnRhinoHitPlayer()
    {
        //camShake.Shake(player.camShakeAmt, player.camShakeLength);
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

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            //Debug.Log("rayOrigin: " + rayOrigin);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                if(hit.collider.tag == "Player")
                {
                    //StartCoroutine(ZoomCamera());
                    OnRhinoHitPlayer();
                    Effect(player.camShakeAmt, player.camShakeLength);
                }
                else if(hit.collider.tag == "Rock")
                {
                    Rock rock = hit.collider.gameObject.GetComponent<Rock>();
                    rock.DestroyObject();
                    Effect(rock.camShakeAmt, rock.camShakeLength);
                }
            }
            rayOrigin += Vector2.down * (horizontalRaySpacing);
        }
    }

    public void Effect(float amt, float length)
    {
        camShake.Shake(amt, length);
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
