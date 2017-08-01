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
    float originalRotation;
    bool isTouchingPlayer = false;

	void Start () {
        FindObjectOfType<Player>().onUnderbrushEvent += OnUnderbrush;
        originalRotation = transform.eulerAngles.z;
        Debug.Log(originalRotation + " " + gameObject.name);
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

    void OnUnderbrush()
    {
        if (isTouchingPlayer)
        {
            Debug.Log("poop");
            StartCoroutine(RotatePlant());
        }
    }

    IEnumerator RotatePlant()
    {
        float targetRotation = originalRotation + rotateAmt;
        Debug.Log("targetRotation " + targetRotation + " transform.eulerAngles.z" + transform.eulerAngles.z);
        // Plant is to the right
        if(player.transform.position.x < raycastPoint.position.x)
        {
            while (transform.eulerAngles.z < targetRotation)
            {
                Debug.Log("targetRotation " + targetRotation + " transform.eulerAngles.z" + transform.eulerAngles.z);
                transform.Rotate(new Vector3(0, 0, 2f));
                yield return null;
            }
            while (transform.eulerAngles.z > originalRotation + 3.1)
            {
                Debug.Log("originalRotation " + originalRotation + " transform.eulerAngles.z" + transform.eulerAngles.z);
                transform.Rotate(new Vector3(0, 0, -1f));
                yield return null;
            }
        }
        else
        {
            while (transform.eulerAngles.z > targetRotation)
            {
                Debug.Log("targetRotation " + targetRotation + " transform.eulerAngles.z" + transform.eulerAngles.z);
                transform.Rotate(new Vector3(0, 0, -2f));
                yield return null;
            }
            while (transform.eulerAngles.z < originalRotation + 3.1)
            {
                Debug.Log("originalRotation " + originalRotation + " transform.eulerAngles.z" + transform.eulerAngles.z);
                transform.Rotate(new Vector3(0, 0, 1f));
                yield return null;
            }
        }
    }
}
