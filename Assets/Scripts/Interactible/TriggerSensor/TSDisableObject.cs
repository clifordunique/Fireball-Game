using UnityEngine;

public class TSDisableObject : TriggerSensor
{
    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            SpriteRenderer[] spriteRenderers = actionObject.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (fade)
                {
                    Utilities.instance.FadeObjectOut(spriteRenderers[i].gameObject, 0.05f, false, false);
                }
                else
                {
                    spriteRenderers[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
