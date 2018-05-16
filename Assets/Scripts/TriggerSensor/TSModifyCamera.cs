using UnityEngine;

public class TSModifyCamera : TriggerSensor {

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            actionObject.GetComponent<CameraFollow>().ModLooking = true;
        }
    }
    public override void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            actionObject.GetComponent<CameraFollow>().ModLooking = false;
        }
    }
}
