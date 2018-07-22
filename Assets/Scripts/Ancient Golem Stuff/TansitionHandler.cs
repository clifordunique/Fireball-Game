using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TansitionHandler : MonoBehaviour {

    Animator animator;

	// Use this for initialization
	void Start () {
        LightOnFire.onFire += EnableTansition;
        animator = GetComponent<Animator>();
	}
	
    void EnableTansition()
    {
        animator.enabled = true;
    }
}
