using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGM : MonoBehaviour {

    GameMaster gm;

    void Awake()
    {
        gm = FindObjectOfType<GameMaster>();
        gm.LoadPlayerStats();
    }
}
