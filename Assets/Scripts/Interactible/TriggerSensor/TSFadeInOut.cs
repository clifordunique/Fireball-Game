using UnityEngine;

public class TSFadeInOut : TriggerSensor
{
    public float speed = 0.08f;
    public bool fadeOutOnEnterTrigger;

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (fadeOutOnEnterTrigger)
            {
                for (int i = 0; i < actionObject.childCount; i++)
                {
                    Transform child = actionObject.GetChild(i);
                    Utilities.instance.FadeObjectOut(child.gameObject, speed, destroy, disable);
                }
                Utilities.instance.FadeObjectOut(actionObject.gameObject, speed, destroy, disable);
                if (destroySelf)
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                actionObject.gameObject.SetActive(true);

                for (int i = 0; i < actionObject.childCount; i++)
                {
                    Transform child = actionObject.GetChild(i);
                    Utilities.instance.FadeObjectIn(child.gameObject, speed);
                }
                Utilities.instance.FadeObjectIn(actionObject.gameObject, speed);
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (fadeOutOnEnterTrigger)
            {
                actionObject.gameObject.SetActive(true);

                for (int i = 0; i < actionObject.childCount; i++)
                {
                    Transform child = actionObject.GetChild(i);
                    Utilities.instance.FadeObjectIn(child.gameObject, speed);
                }
                Utilities.instance.FadeObjectIn(actionObject.gameObject, speed);
            }
            else
            {
                for (int i = 0; i < actionObject.childCount; i++)
                {
                    Transform child = actionObject.GetChild(i);
                    Utilities.instance.FadeObjectOut(child.gameObject, speed, destroy, disable);
                }
                Utilities.instance.FadeObjectOut(actionObject.gameObject, speed, destroy, disable);
                if (destroySelf)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
