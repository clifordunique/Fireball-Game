using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    // Player Stats:
    public float fireHealthDefault = -5f;
    public float? fireHealth = -5f;
    public bool? isFire = null;
    Player player;

    void Awake() // Make it so that bad things don't happen when the level is loaded directly
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
    }

    /* saves the player stats, such as powerups and if he's still on fire
 */
    public void SavePlayerStats()
    {
        //maxHealth = player.maxHealth;
        //maxFireHealth = player.maxFireHealth;
        //fireHealth = player.fireHealth;
        //moveSpeed = player.moveSpeed;
        //maxJumpHeight = player.maxJumpHeight;
        //isFire = player.isFire;
        //Debug.Log(isFire + "  " + fireHealth);
    }

    /* loads the player stats, such as powerups and if he's still on fire
 */
    public void LoadPlayerStats()
    {
        //player.maxHealth  = maxHealth;
        //player.maxFireHealth  = maxFireHealth;
        //player.fireHealth = (float)fireHealth;
        //player.moveSpeed  = moveSpeed;
        //player.maxJumpHeight  = maxJumpHeight;
        //player.isFire = (bool)isFire;
    }
}
