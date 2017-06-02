using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float speed = 30f;

	void Update () {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        Destroy(gameObject, 3f);
	}


}
