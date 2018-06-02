using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplat : MonoBehaviour {

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>(); 
    }
	
	void Update () {
        
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > anim.GetCurrentAnimatorStateInfo(0).length)
        {
            Destroy(gameObject);
        }
        
    }
}
