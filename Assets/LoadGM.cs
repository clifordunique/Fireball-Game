using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGM : MonoBehaviour {

    GameMaster thatPersistingObject;
    void Awake()
    {
        thatPersistingObject = FindObjectOfType<GameMaster>();
        thatPersistingObject.LoadPlayerStats();
    }
}
