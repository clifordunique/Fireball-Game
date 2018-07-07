using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSDisableObject : TriggerSensor {

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            actionObject.gameObject.SetActive(false);
        }
    }
}
