using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Underbrush : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.G))
            GetComponent<SpriteRenderer>().sortingLayerName = "Behind Player";
        if (Input.GetKeyDown(KeyCode.H))
            GetComponent<SpriteRenderer>().sortingLayerName = "Front Player";
    }
}
