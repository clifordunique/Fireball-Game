using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDestroy : MonoBehaviour {

    public GameObject glow;
    SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        sr = glow.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut());
	}

    IEnumerator FadeOut()
    {
        Color tmp = sr.color;
        while (sr.color.a >= 0)
        {
            tmp.a -= 0.1f;
            sr.color = tmp;
            yield return null;
        }

    }
}
