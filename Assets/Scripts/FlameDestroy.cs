using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameDestroy : MonoBehaviour {

    SpriteRenderer sr;
    Rigidbody2D rb2D;
    float yForce;
    float xForce;
    float thrust;

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        StartCoroutine(FadeOut());
        yForce = Random.Range(-0.5f, 2);
        xForce = Random.Range(-1, 0);
        thrust = Random.Range(100, 1000);
        rb2D.AddForce(new Vector2(xForce, yForce) * thrust);
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

    /* If colliding with an enemy or plant, ignore the collision
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Underbrush"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
        }
    }
}
