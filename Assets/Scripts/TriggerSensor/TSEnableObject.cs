using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSEnableObject : TriggerSensor {

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            actionObject.gameObject.SetActive(true);
        }
    }
}
