using System.Collections;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public Utilities.Ambiance CurBackgroundAmbiance;
    public Utilities.State CurState;
    public static GameMaster gm;

    public GameObject endLevelUI;
    
    AudioManager audioManager;

    // Ambiance sounds
    string curAmbianceName; // keeps track of the current ambiance name for convenience
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
    }

    void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
        }
        SaveLoad.Load();
        LoadPlatformSounds();
        PlayBackgroundAmbiance();
    }

    // Restarts the level
    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.FadeOut(2));
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
            if (i >= 9)
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

    /* Uses the audiomanager to play whatever sound matches up with the curAmbianceName.
     */
    void PlayBackgroundAmbiance()
    {
        SetAmbianceName();

        audioManager.PlaySound(curAmbianceName);
    }

    void OnLevelEnd()
    {
        SaveLoad.Save();
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
                Utilities.LoadNextLevel();
                break;
            case 2:
                Utilities.RestartLevel();
                break;
            default:
                Debug.LogError("Not a valid case for FadeOut Function");
                break;
        }
    }

    /// <summary>
    /// Sets the current background ambiance to tha parameter passed in and fades to that ambiance
    /// </summary>
    /// <param name="backgroundAmbiance">The background ambiance to be set in GameMaster</param>
    public void SetAmbianceEnum(Utilities.Ambiance backgroundAmbiance)
    {
        string oldAmbianceName = curAmbianceName;
        CurBackgroundAmbiance = backgroundAmbiance;
        SetAmbianceName();

        FadeBetweenBackgroundAmbiance(oldAmbianceName);
    }

    /// <summary>
    /// Fades between oldAmbianceName out and curAmbianceName in
    /// </summary>
    /// <param name="oldAmbianceName">The ambiance to be faded out</param>
    void FadeBetweenBackgroundAmbiance(string oldAmbianceName)
    {
        audioManager.FadeBetweenTwoSounds(curAmbianceName, oldAmbianceName);
    }

    /// <summary>
    /// Sets curAmbiance name to match up with CurBackgroundAmbiance enum
    /// </summary>
    void SetAmbianceName()
    {
        switch (CurBackgroundAmbiance)
        {
            case Utilities.Ambiance.FOREST:
                curAmbianceName = forestAmbiance;
                break;
            case Utilities.Ambiance.DARKFOREST:
                // play darkforest sound
                break;
            case Utilities.Ambiance.MOUNTAIN:
                curAmbianceName = mountainAmbiance;
                break;
            default:
                Debug.Log("A non-existent ambiance enum was selected in GM.");
                break;
        }
    }
}