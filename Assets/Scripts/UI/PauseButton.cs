using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour {

    public static bool GameIsPaused;

    public GameObject pauseMenuUI;

    Animator anim;
    Attack playerAttack;

	// Use this for initialization
	void Start () {
        playerAttack = FindObjectOfType<Attack>();
        anim = pauseMenuUI.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        playerAttack.enabled = false;
        anim.Play("PauseMenuDown");
    }

    public void Restart()
    {
        Utilities.instance.RestartLevel();
    }

    public void Exit()
    {
        Utilities.instance.Exit();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        playerAttack.enabled = true;
    }
}
