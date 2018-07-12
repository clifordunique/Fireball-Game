using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour
{

    public static Utilities instance;

    GameMaster gm;

    // Use this for initialization
    void Start()
    {
        instance = this;
        gm = GameMaster.gm;
    }

    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        AudioListener.pause = false;
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
    public void Pause()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        gm.CurState = State.PAUSED;
    }

    /// <summary>
    /// Pauses the game, setting timeScale to 1, unpausing the AudioListener, and setting the 
    /// state in GM to running.
    /// </summary>
    public void UnPause()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        gm.CurState = State.RUNNING;
    }

    /// <summary>
    /// Fades and object out
    /// </summary>
    /// <param name="objectToFade">The object to fade out</param>
    /// <param name="destroy">Whether or not to destory the object once it has faded out</param>c
    /// <param name="speed">The fade speed - the amount that the alpha decreases every frame.</param>
    public void FadeObjectOut(GameObject objectToFade, float speed, bool destroy, bool disable)
    {
        StartCoroutine(FadeOut(objectToFade, speed, destroy, disable));
    }

    IEnumerator FadeOut(GameObject objectToFade, float speed, bool destroy, bool disable)
    {
        if (objectToFade != null)
        {
            SpriteRenderer sr = objectToFade.GetComponent<SpriteRenderer>();
            Color tmp = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a);
            float timer = 0;

            while (sr != null && sr.color.a >= 0 && timer < 50)
            {
                timer++;
                tmp.a -= speed;
                sr.color = tmp;
                yield return null;
            }
            if (objectToFade != null)
            {
                if (destroy)
                {
                    Destroy(objectToFade);
                }
                if (disable)
                {
                    objectToFade.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Fades an object in
    /// </summary>
    /// <param name="objectToFade">The object to fade in</param>
    /// <param name="speed">The fade speed - the amount that the alpha increases every frame.</param>
    public void FadeObjectIn(GameObject objectToFade, float speed)
    {
        StartCoroutine(FadeIn(objectToFade, speed));
    }

    IEnumerator FadeIn(GameObject objectToFade, float speed)
    {
        if (objectToFade != null)
        {
            SpriteRenderer sr = objectToFade.GetComponent<SpriteRenderer>();
            Color tmp = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a);
            float timer = 0;

            while (sr != null && sr.color.a <= 1 && timer < 50)
            {
                timer++;
                tmp.a += speed;
                sr.color = tmp;
                yield return null;
            }
        }
    }

    public enum Ambiance { CAVE, FOREST, DARKFOREST, MOUNTAIN };

    public enum PlatformType { GRASS, SNOW, ROCK, WOOD }

    public enum State { PAUSED, RUNNING }

    public enum PowerUps { ZOOM }
}