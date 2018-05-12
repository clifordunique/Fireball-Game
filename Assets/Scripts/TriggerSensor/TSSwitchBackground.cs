using UnityEngine;

public class TSSwitchBackground : TriggerSensor {

    public Utilities.Ambiance backgroundAmbiance;

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameMaster.gm.SetAmbianceEnum(backgroundAmbiance);
        }
    }
}
