using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoHorn : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("inside!!!");
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("one step farther!!!");
            Player player = col.gameObject.GetComponent<Player>();
            player.DamagePlayer(1000);
        }
    }
}
