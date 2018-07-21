using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransporter : MonoBehaviour {

    public Player player;
    public Transform transformToPos;

    Vector2 savedPlayerPos;

	// Use this for initialization
	void Start () {
        LightOnFire.onFire += TransportPlayer;
	}

    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            GoBack();
        }
    }

    void TransportPlayer()
    {
        savedPlayerPos = player.transform.position;
        StartCoroutine(Wait());

        LightOnFire.onFire -= TransportPlayer;
    }

    IEnumerator Wait()
    {
        float waitTime = 1f;
        float targetTime = Time.time + waitTime;

        while (Time.time < targetTime)
        {
            yield return new WaitForSeconds(0.05f);
        }

        player.transform.position = transformToPos.position;
    }

    void GoBack()
    {
        player.transform.position = savedPlayerPos;
    }
}
