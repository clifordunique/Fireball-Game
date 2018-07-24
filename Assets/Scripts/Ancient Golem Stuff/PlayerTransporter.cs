using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransporter : MonoBehaviour {

    public Player player;
    public Transform transformToPos;

    Vector2 savedPlayerPos;
    Animator anim;

	// Use this for initialization
	void Start () {
        LightOnFire.onFire += TransportPlayer;
        anim = GetComponent<Animator>();
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
        float waitTime = 2f;
        float targetTime = Time.time + waitTime;

        while (Time.time < targetTime)
        {
            yield return new WaitForSeconds(0.05f);
        }

        GameMaster.gm.GetComponent<CameraShake>().Shake(0.3f, 2f);
        AudioManager.instance.PlaySound("Shockwave");
        anim.enabled = true;

        targetTime = Time.time + 1f;

        while(Time.time < targetTime)
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
