using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {

    Player player;

	void Start ()
    {
        player = GetComponent<Player>();	
	}
	
	void Update ()
    {
        if (GameMaster.gm.CurState == Utilities.State.PAUSED) return;

        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if(Input.GetButton("Jump"))
        {
            player.OnJumpInputDown();
        }
        if(Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }
        //if (Input.GetButton("Shift"))
        //{
        //    player.OnShiftInput();
        //}
    }
}
