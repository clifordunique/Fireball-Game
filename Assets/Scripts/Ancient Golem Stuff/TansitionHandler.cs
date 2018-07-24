using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TansitionHandler : MonoBehaviour {

    GameMaster gameMaster;
    Animator animator;

	// Use this for initialization
	void Start () {
        LightOnFire.onFire += EnableTansition;
        gameMaster = GameMaster.gm;
        animator = GetComponent<Animator>();
	}
	
    void EnableTansition()
    {
        gameMaster.SetAmbianceEnum(Utilities.Ambiance.MYSTERIOUS);
        gameMaster.SetBackgroundSong(Utilities.Song.NONE);
        animator.enabled = true;
    }
}
