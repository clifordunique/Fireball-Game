using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlur : MonoBehaviour {

    public float fadeOutSpeed = 0.08f;
    public float startAlpha = 1f;

    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Blurs the sprite attached to the script
    /// </summary>
    public void StartBlur()
    {
        CreateSprite();
    }

    void CreateSprite()
    {
        GameObject tempSprite = new GameObject();
        GameObject spriteToFade = Instantiate(tempSprite, new Vector3(transform.position.x, transform.position.y), transform.rotation);
        spriteToFade.AddComponent<SpriteRenderer>();
        spriteToFade.AddComponent<FadeOut>();
        spriteToFade.GetComponent<Transform>().localScale = transform.localScale;

        SpriteRenderer spriteRenderer = spriteToFade.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sr.sprite;

        spriteToFade.GetComponent<FadeOut>().StartFade(startAlpha, fadeOutSpeed);
        //yield return StartCoroutine(FadeOut(spriteRenderer));

        Destroy(tempSprite);
    }

    //IEnumerator FadeOut(SpriteRenderer spriteRenderer)
    //{
    //    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, startAlpha);
    //    Color color = spriteRenderer.color;
    //    while (spriteRenderer.color.a > 0)
    //    {
    //        color = new Color(color.r, color.g, color.b, color.a - fadeOutSpeed);
    //        spriteRenderer.color = color;
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //}
}
