using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PondBasin : MonoBehaviour {

    Animator anim;

	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            anim.SetBool("TriggerEnter", true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            anim.SetBool("TriggerEnter", false);
        }
    }
}
