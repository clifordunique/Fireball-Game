using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameDestroy : MonoBehaviour {

    SpriteRenderer sr;
	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator FadeOut()
    {
        Color tmp = sr.color;
        while(sr.color.a >= 0)
        {
            tmp.a -= 0.02f;
            sr.color = tmp;
            yield return null;
        }

    }

    /* If colliding with an enemy, ignore the collision
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Underbrush"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
        }
    }
}
