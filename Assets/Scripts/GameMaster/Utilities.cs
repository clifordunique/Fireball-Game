using UnityEngine;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour {

    public static Utilities instance;

    GameMaster gm;

	// Use this for initialization
	void Start () {
        instance = this;
        gm = GameMaster.gm;
	}
	
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
        Debug.Log("Exiting Game");
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        gm.CurState = State.PAUSED;
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        gm.CurState = State.RUNNING;
    }

    public enum Ambiance { FOREST, DARKFOREST, MOUNTAIN };

    public enum PlatformType { GRASS, SNOW, ROCK, WOOD }

    public enum State { PAUSED, RUNNING }
}
