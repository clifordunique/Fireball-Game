using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TansitionHandler : MonoBehaviour {

    public Utilities.Song SongToTransitionTo;
    public GameObject enterShineAnimation;
    public GameObject exitShineAnimation;

    GameMaster gameMaster;
    Animator animator;

	// Use this for initialization
	void Start () {
        LightOnFire.onFire += TransitionToMysterious;
        EndMysterious.onEndTransition += TransitionBack;
        gameMaster = GameMaster.gm;
	}
	
    void TransitionToMysterious()
    {
        gameMaster.SetAmbianceEnum(Utilities.Ambiance.MYSTERIOUS);
        gameMaster.SetBackgroundSong(Utilities.Song.NONE);
        enterShineAnimation.GetComponent<Animator>().enabled = true;
        enterShineAnimation.GetComponent<Animator>().Play("ShineThingEnter");
        LightOnFire.onFire -= TransitionToMysterious;
    }

    void TransitionBack()
    {
        gameMaster.SetAmbianceEnum(gameMaster.LevelAmbiance);
        gameMaster.SetBackgroundSong(SongToTransitionTo);
        exitShineAnimation.GetComponent<Animator>().enabled = true;
        exitShineAnimation.GetComponent<Animator>().Play("ShineThingExit");
    }
}
