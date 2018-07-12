using UnityEngine;

public class LightOnFire : MonoBehaviour
{
    public GameObject actionObject;
    public GameObject glow;

    public virtual void FireAction()
    {
        SpriteRenderer[] actionObjects = actionObject.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < actionObjects.Length; i++)
        {
            actionObject.SetActive(true);
            Utilities.instance.FadeObjectIn(actionObjects[i].gameObject, 0.08f);
            Utilities.instance.FadeObjectOut(glow, 0.08f, true, false);
        }
    }
}
