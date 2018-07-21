using UnityEngine;

public class LightOnFire : MonoBehaviour
{
    public GameObject actionObject;
    public GameObject glow;
    public float speed = 0.08f;

    public delegate void OnFire();
    public static event OnFire onFire;

    public virtual void FireAction()
    {
        if(onFire != null)
        {
            onFire();
        }
        SpriteRenderer[] actionObjects = actionObject.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < actionObjects.Length; i++)
        {
            actionObject.SetActive(true);
            Utilities.instance.FadeObjectIn(actionObjects[i].gameObject, speed);
            Utilities.instance.FadeObjectOut(glow, speed, true, false);
        }
    }
}
