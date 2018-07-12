/* This script detects the player and then enables some object
 */
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TriggerSensor : MonoBehaviour {

    public Transform actionObject;
    public bool fade;

    private void Start()
    {
        
    }

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        // Blank
    }

    public virtual void OnTriggerExit2D(Collider2D col)
    {
        // Blank
    }
}
