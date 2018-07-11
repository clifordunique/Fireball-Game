using UnityEngine;

public class LightOnFire : MonoBehaviour
{
    public GameObject actionObject;

    public virtual void FireAction()
    {
        Utilities.instance.FadeObjectIn(actionObject, 0.05f);
    }
}
