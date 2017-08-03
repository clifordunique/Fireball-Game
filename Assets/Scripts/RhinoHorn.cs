using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoHorn : MonoBehaviour {

    Player player;

    void Start()
    {
        FindObjectOfType<Controller2D>().rhinoHitPlayerEvent += OnRhinoHitPlayer;
        player = FindObjectOfType<Player>();
    }

    void OnRhinoHitPlayer()
    {
        Debug.Log("one step farther!!!");
        player.DamagePlayer(1000);
        FindObjectOfType<Controller2D>().rhinoHitPlayerEvent -= OnRhinoHitPlayer;
    }
}
