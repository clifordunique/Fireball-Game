using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhino : MonoBehaviour {

    Player player;
    public Transform head;
    Animator anim;
    public float easeAmount;

    public delegate void OnSeePlayer();
    public event OnSeePlayer seePlayerEvent;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<Player>();
        anim = GetComponent<Animator>();
        seePlayerEvent += SeePlayer;
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(head.rotation.z * Mathf.Rad2Deg);
        //Debug.Log(Mathf.Abs(player.transform.position.x - transform.position.x));
        if (Mathf.Abs(player.transform.position.x - transform.position.x) < 20)
        {
            if (seePlayerEvent != null)
            {
                seePlayerEvent();
            }
        }
	}

    void SeePlayer()
    {
        StartCoroutine(Rotate());
        seePlayerEvent -= SeePlayer;
    }

    IEnumerator Rotate()
    {
        anim.enabled = false;
        float targetRotation = -10;
        float totalRotation = Mathf.Abs(targetRotation - head.rotation.z * Mathf.Rad2Deg);
        float currentRotation;
        float rotationPercentage;

        while (head.rotation.z * Mathf.Rad2Deg > -10)
        {
            currentRotation = Mathf.Abs(targetRotation - head.rotation.z * Mathf.Rad2Deg);
            rotationPercentage = currentRotation / totalRotation;
            rotationPercentage = Mathf.Clamp01(rotationPercentage);
            float easedPercentBetweenRotation = -Ease(rotationPercentage);
            //Debug.Log(head.rotation.z * Mathf.Rad2Deg);
            head.Rotate(Vector3.forward, easedPercentBetweenRotation);
            //Vector3.RotateTowards(head.rotation, targetRotation, easedPercentBetweenRotation, 1f);
            
            yield return null;
        } 
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
}
