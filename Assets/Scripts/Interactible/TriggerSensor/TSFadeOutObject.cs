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
            for(int i = 0; i < actionObject.childCount; i++)
            {
                Utilities.instance.FadeObjectOut(actionObject.GetChild(i).gameObject, 0.05f, destroy, disable);
            }
            Utilities.instance.FadeObjectOut(actionObject.gameObject, 0.05f, destroy, disable);
        }
    }
}
