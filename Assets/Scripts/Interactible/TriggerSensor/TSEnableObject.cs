using UnityEngine;

public class TSEnableObject : TriggerSensor
{

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            actionObject.gameObject.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
