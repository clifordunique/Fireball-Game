using UnityEngine;

public class PauseButton : MonoBehaviour
{

    public GameObject pauseMenuUI;

    public void TogglePauseGame()
    {
        if (Utilities.CurState == Utilities.State.RUNNING)
        {
            pauseMenuUI.SetActive(true);
            Utilities.Pause();
        }
        else
        {
            pauseMenuUI.SetActive(false);
            Utilities.UnPause();
        }
    }

    public void Restart()
    {
        Utilities.RestartLevel();
    }

    public void Exit()
    {
        Utilities.Exit();
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }
}
