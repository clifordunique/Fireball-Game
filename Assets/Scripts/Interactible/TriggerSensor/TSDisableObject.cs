using UnityEngine;
using UnityEngine.UI;

public class TSDisableObject : TriggerSensor
{
    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (actionObject.GetComponentsInChildren<SpriteRenderer>().Length > 0)
            {
                SpriteRenderer[] spriteRenderers = actionObject.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    spriteRenderers[i].gameObject.SetActive(false);
                }
            }
            else if (actionObject.GetComponentsInChildren<Text>().Length > 0)
            {
                Text text = actionObject.GetComponentInChildren<Text>();
                text.enabled = false;
            }
        }
    }
}
