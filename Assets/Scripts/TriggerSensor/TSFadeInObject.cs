using UnityEngine;

public class TSFadeInObject : TriggerSensor {

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            SpriteRenderer[] spriteRenderers = actionObject.GetComponentsInChildren<SpriteRenderer>();
            Debug.Log(spriteRenderers.Length);
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (fade)
                {
                    Utilities.instance.FadeObjectIn(spriteRenderers[i].gameObject, 0.05f);
                }
            }
        }
    }
}
