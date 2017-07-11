using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatOnWater : MonoBehaviour {

    Vector2 velocity;
    float gravity = -18f;

	// Use this for initialization
	void Start () {
        velocity = new Vector2();
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(velocity);
        velocity.y = gravity * Time.deltaTime;
        StartCoroutine(sink());
	}

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.GetComponent<Water>() != null)
        {
            StopAllCoroutines();
            StartCoroutine(rise());
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.GetComponent<Player>() != null)
        {
            velocity.y -= 100;
        }
    }

    IEnumerator sink()
    {
        velocity.y = gravity * Time.deltaTime;
        yield return new WaitForSeconds(1);
    }

    IEnumerator rise()
    {
        velocity.y = 18 * Time.deltaTime;
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(sink());
    }
}
