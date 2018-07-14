using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TSFadeOutObject : TriggerSensor
{
    public bool disable;
    public bool destroy;

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (actionObject.GetComponentsInChildren<SpriteRenderer>().Length > 0)
            {
                SpriteRenderer[] objects = actionObject.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < objects.Length; i++)
                {
                    Utilities.instance.FadeObjectOut(objects[i].gameObject, 0.05f, destroy, disable);
                }
            }
            else if (actionObject.GetComponentsInChildren<Text>().Length > 0)
            {
                Text[] objects = actionObject.GetComponentsInChildren<Text>();
                for (int i = 0; i < objects.Length; i++)
                {
                    Utilities.instance.FadeObjectOut(objects[i].gameObject, 0.05f, destroy, disable);
                }
            }
            else if (actionObject.GetComponentsInChildren<TextMesh>().Length > 0)
            {
                TextMesh[] objects = actionObject.GetComponentsInChildren<TextMesh>();
                for (int i = 0; i < objects.Length; i++)
                {
                    Utilities.instance.FadeObjectOut(objects[i].gameObject, 0.05f, destroy, disable);
                }
            }
        }
    }
}
