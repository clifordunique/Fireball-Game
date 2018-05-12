using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static Enums;

public class GameMaster : MonoBehaviour
{
    public enum ambiance { FOREST, DARKFOREST, SNOW };  // To be used in determining background ambience
    ambiance curBackgroundAmbiance;

    public static GameMaster gm;

    public GameObject endLevelUI;
    
    AudioManager audioManager;

    //private static ambience curAmbience;
    public string snowBackground;
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
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
        }
    }

    void Start()
    {
        

        LoadPlatformSounds();
        GetRandomIndex();
        PlayBackgroundMusic();
    }

    //void LateUpdate()
    //{
    //    if (!audioManager.isPlaying(forestBackgroundArray[backgroundSoundIndex]))
    //    {
    //        GetRandomIndex();
    //        PlayBackgroundMusic();
    //    }
    //}

    // Restarts the level
    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.FadeOut(2));
    }

    /* Sets the curBackgroundAmbiance to whatever is passed in as a string
     * from @backgroundAmbiance.
     */
    public void SetAmbianceEnum(string backgroundAmbiance)
    {
        switch (backgroundAmbiance)
        {
            case "mountainwind":
                curBackgroundAmbiance = ambiance.SNOW;
                PlayBackgroundMusic();
                break;
            case "dark forest":
                curBackgroundAmbiance = ambiance.DARKFOREST;
                break;
            case "forest":
                curBackgroundAmbiance = ambiance.FOREST;
                PlayBackgroundMusic();
                break;
            default:
                Debug.Log("poop");
                break;

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
        switch (curBackgroundAmbiance)
        {
            case ambiance.FOREST:
                GetRandomIndex();
                audioManager.PlaySound(forestBackgroundArray[backgroundSoundIndex]);
                break;
            case ambiance.DARKFOREST:
                // play darkforest sound
                break;
            case ambiance.SNOW:
                audioManager.Fade(snowBackground, forestBackgroundArray[backgroundSoundIndex]);
                //audioManager.StopSound(forestBackgroundArray[backgroundSoundIndex]);
                //audioManager.PlaySound(snowBackground);
                break;
            default:
                Debug.Log("A non-existent ambiance enum was selected in GM.");
                break;
        }
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
                Utilities.instance.LoadNextLevel();
                break;
            case 2:
                Utilities.instance.RestartLevel();                
                break;
            default:
                Debug.LogError("Not a valid case for FadeOut Function");
                break;
        }
    }
    /*
    IEnumerator waitToLoad(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene("Level03");

    }
    */
}