using UnityEngine;

public class TSFadeInOut : TriggerSensor
{

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            actionObject.gameObject.SetActive(true);
            Utilities.instance.FadeObjectIn(actionObject.gameObject, 0.08f);

            for (int i = 0; i < actionObject.childCount; i++)
            {
                Transform child = actionObject.GetChild(i);
                Utilities.instance.FadeObjectIn(child.gameObject, 0.08f);
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Utilities.instance.FadeObjectOut(actionObject.gameObject, 0.08f, false, false);

            for (int i = 0; i < actionObject.childCount; i++)
            {
                Transform child = actionObject.GetChild(i);
                Utilities.instance.FadeObjectOut(child.gameObject, 0.08f, false, false);
            }
        }
    }
}
