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
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if(Input.GetButtonDown("Jump") && !Input.GetButton("Shift"))
        {
            player.OnJumpInputDown();
        }
        if(Input.GetButtonUp("Jump") && !Input.GetButton("Shift"))
        {
            player.OnJumpInputUp();
        }
        if (Input.GetButton("Shift"))
        {
            player.OnShiftInput();
        }
    }
}
