using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSEnablePowerups : TriggerSensor {

    public Utilities.PowerUps powerups;

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerStats.instance.EnablePowerup(powerups);
        }
    }
}
