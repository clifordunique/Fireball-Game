using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.up * 20 * Time.deltaTime);
        Destroy(gameObject, 5f);
	}
}
