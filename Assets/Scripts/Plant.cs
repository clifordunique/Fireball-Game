using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

    public LayerMask playerLayerMask;
    float originalRotation;
    bool isTouchingPlayer = false;

	void Start () {
        FindObjectOfType<Player>().onUnderbrushEvent += OnUnderbrush;
        originalRotation = transform.eulerAngles.z;
        Debug.Log(originalRotation);
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 5f, playerLayerMask);
        if (hit)
        {
            Debug.Log("is touching player");
            isTouchingPlayer = true;
            Debug.Log("is touching player" + isTouchingPlayer);
        }
        else
        {
            isTouchingPlayer = false;
            Debug.Log("is touching playe falser" + isTouchingPlayer);
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
        float targetRotation = originalRotation + 5;
        Debug.Log("targetRotation " + targetRotation + " transform.eulerAngles.z" + transform.eulerAngles.z);
        while (transform.eulerAngles.z < targetRotation)
        {
            Debug.Log("targetRotation " + targetRotation + " transform.eulerAngles.z" + transform.eulerAngles.z);
            transform.Rotate(new Vector3(0, 0, 3f));
            yield return null;
        }
        while (transform.eulerAngles.z > originalRotation + 3.1)
        {
            Debug.Log("originalRotation " + originalRotation + " transform.eulerAngles.z" + transform.eulerAngles.z);
            transform.Rotate(new Vector3(0, 0, -1f));
            yield return null;
        }
    }
}
