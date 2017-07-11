using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour {

    public GameObject fire;
    public float displacementX;
    public float displacementY;

    public delegate void OnLevelEnd();
    public event OnLevelEnd levelEndEvent;

    GameMaster gm;
	
    void Start()
    {
        gm = FindObjectOfType<GameMaster>();
    }

	void Update () {
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null && col.GetComponent<Player>().isFire)
        {
            Instantiate(fire, new Vector2(transform.position.x + displacementX, transform.position.y + displacementY), transform.rotation);
            if (levelEndEvent != null)
            {
                levelEndEvent();
            }
        }
    }
}
