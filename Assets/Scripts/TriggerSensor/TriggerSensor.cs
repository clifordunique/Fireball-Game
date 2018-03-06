/* This script detects the player and then enables some object
 */
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class TriggerSensor : MonoBehaviour {

    public Transform actionObject;

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        // Blank
    }

    public virtual void OnTriggerExit2D(Collider2D col)
    {
        // Blank
    }
}
