using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFade : MonoBehaviour {

    public Sprite[] sprites;
    public Transform side1, side2;

    int arraySize;
    int index;
    float dstBtwSides;
    Player player;
    Animator anim;

    SpriteRenderer sr;

    void Start()
    {
        dstBtwSides = Mathf.Abs(side2.position.x - side1.position.x);
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.GetComponent<Player>() != null)
        {
            anim.SetFloat("Distance", 1);
        }
    }
    /*
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            anim.SetFloat("Distance", 0);
        }
    }
    
    IEnumerator FirstThing(Player player)
    {
        float oldPlayerPos = player.transform.position.x;
        while (true)
        {
            if (oldPlayerPos != player.transform.position.x)
            {
                float percentageBetweenSides = (Mathf.Abs(player.transform.position.x - side1.position.x) / dstBtwSides);
                int pos = (int)(percentageBetweenSides * sprites.Length);
                index = pos;
                Debug.Log("before " + index);
                StartCoroutine(Something(index, index + 20));
            }
            yield return null;
        }
    }

    IEnumerator Something(float index, float targetIndex)
    {
        while(index < targetIndex)
        {
            index = Mathf.MoveTowards(index, targetIndex, 1);
            sr.sprite = sprites[(int)index];
            Debug.Log("After " + index);
            yield return null;
        }

    }
    */
}
