using System.Collections;
using UnityEngine;

public class TSEnableDisableObject : TriggerSensor
{

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            actionObject.gameObject.SetActive(true);
        }
    }

    public override void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            actionObject.gameObject.SetActive(false);
        }
    }
}
