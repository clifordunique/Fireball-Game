using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

    public LayerMask playerLayerMask;
    public float dstRightLeft;
    public float dstUpDown;
    public Transform raycastPoint;
    public float rotateAmt = 8;

    Player player;
    bool isTouchingPlayer = false;

	void Start () {
        FindObjectOfType<Player>().onUnderbrushEvent += OnUnderbrush;
        //FindObjectOfType<Player>().fingPoop += Crap;
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        RaycastHit2D hitDown = Physics2D.Raycast(raycastPoint.position, Vector2.down, dstUpDown, playerLayerMask);
        RaycastHit2D hitUp = Physics2D.Raycast(raycastPoint.position, Vector2.up, dstUpDown, playerLayerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(raycastPoint.position, Vector2.right, dstRightLeft, playerLayerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(raycastPoint.position, Vector2.left, dstRightLeft, playerLayerMask);

        Debug.DrawRay(raycastPoint.position, Vector3.down * dstUpDown);
        Debug.DrawRay(raycastPoint.position, Vector3.up * dstUpDown);
        Debug.DrawRay(raycastPoint.position, Vector3.left * dstRightLeft);
        Debug.DrawRay(raycastPoint.position, Vector3.right * dstRightLeft);
        if (hitDown || hitRight || hitLeft)
        {
            isTouchingPlayer = true;
            //Debug.Log("is touching player" + isTouchingPlayer);
        }
        else
        {
            isTouchingPlayer = false;
            //Debug.Log("is touching playe falser" + isTouchingPlayer);
        }
    }

    //void Crap()
    //{
    //    Debug.Log("incrap");
    //}

    void OnUnderbrush()
    {
        if (isTouchingPlayer)
        {
            StartCoroutine(RotatePlant());
        }
    }

    IEnumerator RotatePlant()
    {
        // Plant is to the right
        if(player.transform.position.x < raycastPoint.position.x)
        {
            //float targetRotation = originalRotation + rotateAmt;
            //float eulerAngles = transform.eulerAngles.z + 90;

            float deltaRotation = 0;

            while (deltaRotation < rotateAmt)
            {
                //Debug.Log("deltaRotation " + deltaRotation + ". rotateAmt " + (rotateAmt));
                transform.Rotate(new Vector3(0, 0, 1f));
                deltaRotation += 1f;
                yield return null;
            }
            while (deltaRotation > 0)
            {
                //Debug.Log("deltaRotation " + deltaRotation + " rotateAmt " + (rotateAmt));
                transform.Rotate(new Vector3(0, 0, -1f));
                deltaRotation -= 1f;
                yield return null;
            }
        }
        else // plant is to the left
        {
            //float targetRotation = originalRotation - rotateAmt;
            //float eulerAngles = transform.eulerAngles.z + 90;

            float deltaRotation = 0;

            while (deltaRotation > -rotateAmt)
            {
                //Debug.Log("deltaRotation " + deltaRotation + ". rotateAmt " + (rotateAmt));
                transform.Rotate(new Vector3(0, 0, -1f));
                deltaRotation -= 1f;
                yield return null;
            }
            while (deltaRotation < 0)
            {
                //Debug.Log("deltaRotation " + deltaRotation + ". rotateAmt " + (rotateAmt));
                transform.Rotate(new Vector3(0, 0, 1f));
                deltaRotation += 1f;
                yield return null;
            }
        }
    }
}
