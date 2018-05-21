using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStarter : MonoBehaviour {

    public Utilities.Ambiance backgroundAmbiance;

	// Use this for initialization
	void Start () {
        new WaitForSeconds(0.1f);
        GameMaster.gm.SetAmbianceEnum(backgroundAmbiance);
	}
}
