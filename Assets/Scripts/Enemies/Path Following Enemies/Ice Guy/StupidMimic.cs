using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidMimic : MonoBehaviour {

    public Transform mimicObject;

	void Update () {
        mimicObject.position = transform.position + Vector3.up;
        mimicObject.rotation = transform.rotation;
        mimicObject.localScale = transform.localScale;
        mimicObject.eulerAngles += new Vector3(mimicObject.rotation.x, mimicObject.rotation.y, mimicObject.rotation.z + 90);
	}
}
