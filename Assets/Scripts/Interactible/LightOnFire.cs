using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOnFire : MonoBehaviour
{
    public GameObject glowObject;
    public GameObject actionObject;

    protected virtual void Start()
    {
        Color color = glowObject.GetComponent<SpriteRenderer>().color;
        glowObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Utilities.instance.FadeObjectIn(glowObject, 0.05f);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Utilities.instance.FadeObjectOut(glowObject, 0.05f, false, false);
        }
    }

    public virtual void FireAction()
    {
        Utilities.instance.FadeObjectIn(actionObject, 0.05f);
    }
}
