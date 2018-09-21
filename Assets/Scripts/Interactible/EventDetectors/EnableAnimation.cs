using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAnimation : MonoBehaviour {

    Animator anim;

	void Start () {
        anim = GetComponent<Animator>();
        IceLegMove.onAllLegsDead += OnAllLegsDead;
	}
	
    private void OnAllLegsDead()
    {
        anim.enabled = true;
    }
}
