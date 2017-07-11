using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGM : MonoBehaviour {

    GameMaster gm;

    void Awake() // Make it so that bad things don't happen when the level is loaded directly
    {
        gm = FindObjectOfType<GameMaster>();
        gm.LoadPlayerStats();
    }
}
