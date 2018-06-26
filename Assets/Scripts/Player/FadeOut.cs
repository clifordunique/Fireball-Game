using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{

    public void StartFade(float startAlpha, float fadeOutSpeed)
    {
        StartCoroutine(FadeOutRoutine(startAlpha, fadeOutSpeed));
    }

    IEnumerator FadeOutRoutine(float startAlpha, float fadeOutSpeed)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, startAlpha);
        Color color = sr.color;
        while (sr.color.a > 0)
        {
            color = new Color(color.r, color.g, color.b, color.a - fadeOutSpeed);
            sr.color = color;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(this.gameObject);
    }
}
