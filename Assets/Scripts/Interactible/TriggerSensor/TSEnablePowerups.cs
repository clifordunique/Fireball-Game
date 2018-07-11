using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSEnablePowerups : TriggerSensor {

    public Utilities.PowerUps powerups;
    public Vector2 spriteOffset;

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerStats.instance.EnablePowerup(powerups);
            actionObject.gameObject.SetActive(true);
            //Instantiate(actionObject, new Vector2(transform.position.x + spriteOffset.x, transform.position.y + spriteOffset.y), transform.rotation);
            //Destroy(this.gameObject);
        }
    }
}
