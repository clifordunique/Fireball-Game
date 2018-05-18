using UnityEngine;

public class PauseButton : MonoBehaviour
{

    public GameObject pauseMenuUI;

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
        if (GameMaster.gm.CurState == Utilities.State.RUNNING)
        {
            pauseMenuUI.SetActive(true);
            if (playerAttack != null)
                playerAttack.enabled = false;
            Utilities.instance.Pause();
        }
        else
        {
            pauseMenuUI.SetActive(false);
            if (playerAttack != null)
                playerAttack.enabled = true;
            Utilities.instance.UnPause();
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
