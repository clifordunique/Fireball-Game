using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlur : MonoBehaviour
{
    const string playerLayerName = "Player";
    public float fadeOutSpeed = 0.08f;
    public float startAlpha = 1f;
    public int orderInLayer = -2;

    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
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
        spriteRenderer.sprite = this.spriteRenderer.sprite;
        spriteRenderer.sortingLayerName = playerLayerName;
        spriteRenderer.sortingOrder = orderInLayer;

        spriteToFade.GetComponent<FadeOut>().StartFade(startAlpha, fadeOutSpeed);

        Destroy(tempSprite);
    }
}
