using UnityEngine;

public class TSFadeInOut : TriggerSensor {

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Utilities.instance.FadeObjectIn(actionObject.gameObject, 0.05f);
        }
    }

    public override void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Utilities.instance.FadeObjectOut(actionObject.gameObject, 0.05f, false, false);
        }
    }
}
