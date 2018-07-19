using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Utilities : MonoBehaviour
{

    public static Utilities instance;

    GameMaster gm;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
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
        if (objectToFade != null)
        {
            if (objectToFade.GetComponent<TextMesh>())
            {
                StartCoroutine(FadeOut(objectToFade.GetComponent<TextMesh>(), speed, destroy, disable));
            }
            else if (objectToFade.GetComponent<SpriteRenderer>())
            {
                StartCoroutine(FadeOut(objectToFade.GetComponent<SpriteRenderer>(), speed, destroy, disable));
            }
            else if (objectToFade.GetComponent<Text>())
            {
                StartCoroutine(FadeOut(objectToFade.GetComponent<Text>(), speed, destroy, disable));
            }
            else
            {
                Debug.Log("The object fading out doesn't have the correct objects attached.");
            }
        }
    }

    IEnumerator FadeOut(TextMesh objectToFade, float speed, bool destroy, bool disable)
    {
        if (objectToFade != null)
        {
            Color tmp = new Color(objectToFade.color.r, objectToFade.color.g, objectToFade.color.b, objectToFade.color.a);
            float timer = 0;

            while (objectToFade != null && objectToFade.color.a >= 0 && timer < 50)
            {
                timer++;
                tmp.a -= speed;
                objectToFade.color = tmp;
                yield return null;
            }
            if (objectToFade != null)
            {
                if (destroy)
                {
                    Destroy(objectToFade.gameObject);
                }
                if (disable)
                {
                    objectToFade.gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator FadeOut(SpriteRenderer objectToFade, float speed, bool destroy, bool disable)
    {
        if (objectToFade != null)
        {
            Color tmp = new Color(objectToFade.color.r, objectToFade.color.g, objectToFade.color.b, objectToFade.color.a);
            float timer = 0;

            while (objectToFade != null && objectToFade.color.a >= 0 && timer < 50)
            {
                timer++;
                tmp.a -= speed;
                objectToFade.color = tmp;
                yield return null;
            }
            if (objectToFade != null)
            {
                if (destroy)
                {
                    Destroy(objectToFade.gameObject);
                }
                if (disable)
                {
                    objectToFade.gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator FadeOut(Text objectToFade, float speed, bool destroy, bool disable)
    {
        if (objectToFade != null)
        {
            Color tmp = new Color(objectToFade.color.r, objectToFade.color.g, objectToFade.color.b, objectToFade.color.a);
            float timer = 0;

            while (objectToFade != null && objectToFade.color.a >= 0 && timer < 50)
            {
                timer++;
                tmp.a -= speed;
                objectToFade.color = tmp;
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
                    objectToFade.enabled = false;
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
        if (objectToFade != null)
        {
            if (objectToFade.GetComponent<TextMesh>())
            {
                StartCoroutine(FadeIn(objectToFade.GetComponent<TextMesh>(), speed));
            }
            else if (objectToFade.GetComponent<SpriteRenderer>())
            {
                StartCoroutine(FadeIn(objectToFade.GetComponent<SpriteRenderer>(), speed));
            }
            else if (objectToFade.GetComponent<Text>())
            {
                StartCoroutine(FadeIn(objectToFade.GetComponent<Text>(), speed));
            }
            else
            {
                Debug.Log("The object fading in doesn't have the correct objects attached.");
            }
        }
    }

    IEnumerator FadeIn(TextMesh objectToFade, float speed)
    {
        if (objectToFade != null)
        {
            Color tmp = new Color(objectToFade.color.r, objectToFade.color.g, objectToFade.color.b, objectToFade.color.a);
            float timer = 0;

            while (objectToFade != null && objectToFade.color.a <= 1 && timer < 50)
            {
                timer++;
                tmp.a += speed;
                objectToFade.color = tmp;
                yield return null;
            }
        }
    }

    IEnumerator FadeIn(SpriteRenderer objectToFade, float speed)
    {
        if (objectToFade != null)
        {
            Color tmp = new Color(objectToFade.color.r, objectToFade.color.g, objectToFade.color.b, objectToFade.color.a);
            float timer = 0;

            while (objectToFade != null && objectToFade.color.a <= 1 && timer < 50)
            {
                timer++;
                tmp.a += speed;
                objectToFade.color = tmp;
                yield return null;
            }
        }
    }

    IEnumerator FadeIn(Text objectToFade, float speed)
    {
        if (objectToFade != null)
        {
            Color tmp = new Color(objectToFade.color.r, objectToFade.color.g, objectToFade.color.b, objectToFade.color.a);
            float timer = 0;

            while (objectToFade != null && objectToFade.color.a <= 1 && timer < 50)
            {
                timer++;
                tmp.a += speed;
                objectToFade.color = tmp;
                yield return null;
            }
        }
    }

    public enum Ambiance { CAVE, FOREST, DARKFOREST, MOUNTAIN };

    public enum PlatformType { GRASS, SNOW, ROCK, WOOD, MUD }

    public enum State { PAUSED, RUNNING }

    public enum PowerUps { ZOOM, SHOOT }
}