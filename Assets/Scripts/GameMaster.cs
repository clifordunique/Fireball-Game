using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{

    public static GameMaster gm;

    public GameObject endLevelUI;

    AudioManager audioManager;
    public string[] forestBackgroundArray;
    string[] woodCrackClips;
    string[] grassPlatformAudioClips;
    string audioClip;
    int backgroundSoundIndex;

    void Awake()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
        //if (FindObjectOfType<Player>() != null)
        //{
        //    //FindObjectOfType<Player>().fingPoop += KillPlayer;
        //}
        //if (FindObjectOfType<Controller2D>() != null)
        //{
        //    FindObjectOfType<Controller2D>().hitBranchEvent += OnHitBranch;
        //}
        if (FindObjectOfType<CampFire>() != null)
        {
            FindObjectOfType<CampFire>().levelEndEvent += OnLevelEnd;
        }
    }

    void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
        }

        LoadPlatformSounds();
        GetRandomIndex();
        PlayBackgroundMusic();
    }

    // Restarts the level
    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.FadeOut(2));
    }

    void LateUpdate()
    {
        if (!audioManager.isPlaying(forestBackgroundArray[backgroundSoundIndex]))
        {
            GetRandomIndex();
            PlayBackgroundMusic();
        }
    }

    /* Method for loading all the needed platform movement
     *  audioclips into arrays
     */
    void LoadPlatformSounds()
    {
        grassPlatformAudioClips = new string[14];
        for (int i = 0; i < grassPlatformAudioClips.Length; i++)
        {
            if (i < 9)
                grassPlatformAudioClips[i] = "grass" + "0" + (i + 1);
            else
                grassPlatformAudioClips[i] = "grass" + (i + 1);
        }
    }

    public void PlayPlatformAudio(int platformIndex)
    {
        if (!audioManager.isPlaying(audioClip) || audioClip == null)
        {
            switch (platformIndex)
            {
                case 0:
                    audioClip = grassPlatformAudioClips[Random.Range(0, grassPlatformAudioClips.Length)];
                    audioManager.PlaySound(audioClip);
                    break;
                default:
                    Debug.Log("something otehr than grass playing");
                    break;
            }
        }
    }

    void GetRandomIndex()
    {
        backgroundSoundIndex = Random.Range(0, forestBackgroundArray.Length);
    }

    void PlayBackgroundMusic()
    {
        audioManager.PlaySound(forestBackgroundArray[backgroundSoundIndex]);
    }

    void OnLevelEnd()
    {
        StartCoroutine(FadeOut(1));
        FindObjectOfType<CampFire>().levelEndEvent -= OnLevelEnd;
    }

    IEnumerator FadeOut(int action)
    {
        yield return new WaitForSeconds(2);
        endLevelUI.SetActive(true);
        yield return new WaitForSeconds(2);
        switch (action)
        {
            case 1:
                LevelEnd();
                break;
            case 2:
                RestartLevel();
                break;
            default:
                Debug.LogError("Not a valid case for FadeOut Function");
                break;
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LevelEnd()
    {
        //loader.SavePlayerStats()
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    /*
    IEnumerator waitToLoad(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene("Level03");

    }
    */
}