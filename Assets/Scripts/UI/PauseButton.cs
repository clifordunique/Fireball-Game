using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{

    public static bool GameIsPaused;

    public GameObject pauseMenuUI;

    bool isPaused = false;
    Attack playerAttack;

    // Use this for initialization
    void Start()
    {
        playerAttack = FindObjectOfType<Attack>();
    }

    public void TogglePauseGame()
    {
        if (!GameIsPaused)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
            playerAttack.enabled = false;
        }
        else
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
            playerAttack.enabled = true;
        }
    }

    public void Restart()
    {
        Utilities.instance.RestartLevel();
    }

    public void Exit()
    {
        Utilities.instance.Exit();
    }
}
