using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    //public enum ambiance { FOREST, DARKFOREST, SNOW };  // To be used in determining background ambience
    public Utilities.Ambiance CurBackgroundAmbiance{ get; set; }
    public Utilities.State CurState { get; set; }
    public static GameMaster gm;

    public GameObject endLevelUI;
    
    AudioManager audioManager;

    // Ambiance sounds
    string curAmbiance; // keeps track of the current ambiance for convenience
    public string mountainAmbiance;
    public string forestAmbiance;


    string[] woodCrackClips;
    string[] grassPlatformAudioClips;
    string audioClip;
    int backgroundSoundIndex;

    void Awake()
    {
        CurState = Utilities.State.RUNNING;
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
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
        PlayBackgroundAmbiance();
    }

    // Restarts the level
    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.FadeOut(2));
    }

    /// <summary>
    /// Sets the current background ambiance to tha parameter passed in
    /// </summary>
    /// <param name="backgroundAmbiance">The background ambiance to be set in GameMaster</param>
    public void SetAmbianceEnum(Utilities.Ambiance backgroundAmbiance)
    {
        CurBackgroundAmbiance = backgroundAmbiance;
        PlayBackgroundAmbiance();
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
            if (i > 9)
                grassPlatformAudioClips[i] = "grass" + (i + 1);
        }
    }

    public void PlayPlatformAudio(Utilities.PlatformType platformIndex)
    {
        if (!audioManager.isPlaying(audioClip) || audioClip == null)
        {
            switch (platformIndex)
            {
                case Utilities.PlatformType.GRASS:
                    audioClip = grassPlatformAudioClips[Random.Range(0, grassPlatformAudioClips.Length)];
                    audioManager.PlaySound(audioClip);
                    break;
                case Utilities.PlatformType.ROCK:
                    break;
                case Utilities.PlatformType.SNOW:
                    break;
                case Utilities.PlatformType.WOOD:
                    break;
                default:
                    Debug.Log("Some other index got sent through somehow...");
                    break;
            }
        }
    }

    /* Uses the audiomanager to play whatever sound matches up with the CurBackgroundAmbiance enum.
     */
    void PlayBackgroundAmbiance()
    {
        switch (CurBackgroundAmbiance)
        {
            case Utilities.Ambiance.FOREST:
                //GetRandomIndex();
                audioManager.PlaySound(forestAmbiance);
                curAmbiance = forestAmbiance;
                break;
            case Utilities.Ambiance.DARKFOREST:
                // play darkforest sound
                break;
            case Utilities.Ambiance.MOUNTAIN:
                audioManager.Fade(mountainAmbiance, curAmbiance);
                curAmbiance = mountainAmbiance;
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
}