using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public Utilities.Ambiance CurBackgroundAmbiance;
    public Utilities.State CurState { get; set; }
    public static GameMaster gm;

    public GameObject endLevelUI;

    AudioManager audioManager;
    Stopwatch sw;

    // Ambiance sounds
    string curAmbianceName; // keeps track of the current ambiance name for convenience
    public string mountainAmbiance;
    public string forestAmbiance;

    // Footstep sounds
    public string stoneWoodSound;

    string[] woodCrackClips;
    string[] grassPlatformAudioClips;
    string audioClip;
    int backgroundSoundIndex;

    public float timeBetweenFootsteps;
    float timer;

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
            UnityEngine.Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
        }
        SaveLoad.Load();
        LoadPlatformSounds();
        PlayBackgroundAmbiance();
        sw = new Stopwatch();
    }

    private void Update()
    {
        if(sw.ElapsedMilliseconds > 600)
        {
            sw.Start();
            sw.Reset();
        }
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

    /// <summary>
    /// Plays platform audio which changes speed based on the player's velocity.
    /// </summary>
    /// <param name="platformType">The current platform type</param>
    /// <param name="velocityMagnitude">The player's current magnitued</param>
    /// <param name="moveSpeed">The pre-set moveSpeed for the player</param>
    public void PlayPlatformAudio(Utilities.PlatformType platformType, float velocityMagnitude, float moveSpeed)
    {
        float millisecondsMultiplier = moveSpeed / velocityMagnitude;

        if (sw.ElapsedMilliseconds > 400 * millisecondsMultiplier || !sw.IsRunning)
        {
            switch (platformType)
            {
                case Utilities.PlatformType.GRASS:
                    audioClip = grassPlatformAudioClips[Random.Range(0, grassPlatformAudioClips.Length)];
                    break;
                case Utilities.PlatformType.ROCK:
                    audioClip = "";
                    break;
                case Utilities.PlatformType.SNOW:
                    audioClip = "";
                    break;
                case Utilities.PlatformType.WOOD:
                    audioClip = stoneWoodSound;
                    break;
                default:
                    audioClip = "";
                    break;
            }
            if (audioClip != "")
            {
                audioManager.PlaySound(audioClip);
            }
            sw.Reset();
            sw.Start();
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
                UnityEngine.Debug.LogError("Not a valid case for FadeOut Function");
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
                UnityEngine.Debug.Log("A non-existent ambiance enum was selected in GM.");
                break;
        }
    }
}