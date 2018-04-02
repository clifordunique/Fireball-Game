using UnityEngine;

public class PauseButton : MonoBehaviour
{

    public static bool GameIsPaused;

    public GameObject pauseMenuUI;

    bool isPaused = false;
    Attack playerAttack;

    // Use this for initialization
    void Awake()
    {
        playerAttack = FindObjectOfType<Attack>();

        if (playerAttack == null)
        {
            Debug.Log("No playerAttack found");
        }
    }

    public void TogglePauseGame()
    {
        if (!GameIsPaused)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
            if (playerAttack != null)
                playerAttack.enabled = false;
        }
        else
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
            if (playerAttack != null)
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

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }
}
