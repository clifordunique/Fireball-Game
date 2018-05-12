using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStarter : MonoBehaviour {

    public string backgroundAmbiance;

	// Use this for initialization
	void Start () {
        GameMaster.gm.SetAmbianceEnum(backgroundAmbiance);
	}
}
