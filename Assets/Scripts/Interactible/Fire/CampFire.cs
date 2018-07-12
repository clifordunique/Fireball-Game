using UnityEngine;
using System.Collections;

public class CampFire : LightOnFire {

    public delegate void OnLevelEnd();
    public event OnLevelEnd levelEndEvent;

    public override void FireAction()
    {
        base.FireAction();
        if (levelEndEvent != null)
        {
            levelEndEvent();
        }
        Destroy(this); // Added so you can't light two fires
    }
}