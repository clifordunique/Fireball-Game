using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStarter : MonoBehaviour {

    public Utilities.Ambiance backgroundAmbiance;

	// Use this for initialization
	void Start () {
        GameMaster.gm.SetAmbianceEnum(backgroundAmbiance);
	}
}
