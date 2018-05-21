using UnityEngine;

public class PauseButton : MonoBehaviour
{

    public GameObject pauseMenuUI;

    public void TogglePauseGame()
    {
        if (GameMaster.gm.CurState == Utilities.State.RUNNING)
        {
            pauseMenuUI.SetActive(true);
            Utilities.instance.Pause();
        }
        else
        {
            pauseMenuUI.SetActive(false);
            Utilities.instance.UnPause();
        }
    }

    public void Restart()
    {
        Utilities.instance.RestartLevel();
    }

    public void Exit()
    {
        SaveLoad.Save();
        Utilities.instance.Exit();
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }
}
