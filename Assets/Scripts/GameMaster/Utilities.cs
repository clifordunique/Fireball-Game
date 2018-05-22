using UnityEngine;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour {

    public static Utilities instance;

    public static State CurState = State.RUNNING;

 //   GameMaster gm;

	//// Use this for initialization
	//void Start () {
 //       instance = this;
 //       gm = GameMaster.gm;
	//}
	
    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public static void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public static void Exit()
    {
        Debug.Log("Exiting Game");
    }

    /// <summary>
    /// Pauses the game, setting timeScale to 0, pausing the AudioListener, and setting the 
    /// state in GM to paused.
    /// </summary>
    public static void Pause()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        CurState = State.PAUSED;
    }

    /// <summary>
    /// Pauses the game, setting timeScale to 1, unpausing the AudioListener, and setting the 
    /// state in GM to running.
    /// </summary>
    public static void UnPause()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        CurState = State.RUNNING;
    }

    public enum Ambiance { FOREST, DARKFOREST, MOUNTAIN };

    public enum PlatformType { GRASS, SNOW, ROCK, WOOD }

    public enum State { PAUSED, RUNNING }

    public enum PowerUps { ZOOM }
}
