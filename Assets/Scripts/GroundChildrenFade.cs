using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChildrenFade : MonoBehaviour {

    Animator anim;

    public float distanceForward;
    public float distanceExit;

    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            anim.SetFloat("Distance", distanceForward);
        }
    }
    /*
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            anim.SetFloat("Distance", distanceExit);
        }
    }
    */
}
