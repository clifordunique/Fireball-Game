using UnityEngine;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour {

    public static Utilities instance;

	// Use this for initialization
	void Start () {
        instance = this;
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

    public enum Ambiance { FOREST, DARKFOREST, MOUNTAIN };

    public enum PlatformType { GRASS, SNOW, ROCK, WOOD }
}
