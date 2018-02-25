/* This script detects the player and then enables some object
 */
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class TriggerSensor : MonoBehaviour {

    public Transform objectToBeEnabled;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.GetComponent<Player>() != null)
        {
            Debug.Log("It has happened");
            objectToBeEnabled.gameObject.SetActive(true);
        }
    }
}
