using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(requiredComponent: typeof(SceneStarter))]
public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    public Utilities.Ambiance LevelAmbiance;

    public Utilities.State CurState { get; set; }
    private Utilities.Ambiance CurBackgroundAmbiance;
    public Utilities.Song CurSong;

    public GameObject endLevelUI;

    SceneStarter sceneStarter;
    AudioManager audioManager;
    Stopwatch sw;

    // Ambiance sounds
    string curAmbianceName; // keeps track of the current ambiance name for convenience
    public string mountainAmbiance;
    public string forestAmbiance;
    public string caveAmbiance;
    public string mysteriousAmbiance;

    // Background songs
    string curSongName;
    public string level01Song;

    // Footstep sounds
    public string woodPlatformAudioClip;
    public string mudPlatformAudioClip;
    public string snowPlatformAudioClip;
    public string mysteriousPlatformAudioClip;

    string[] woodCrackClips;
    string[] grassPlatformAudioClips;
    string audioClip;
    int backgroundSoundIndex;

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
        sceneStarter = GetComponent<SceneStarter>();

        CurBackgroundAmbiance = LevelAmbiance;

        SaveLoad.Load();
        LoadPlatformSounds();

        PlayBackgroundSong();
        PlayBackgroundAmbiance();
        sw = new Stopwatch();
    }

    private void Update()
    {
        if (sw.ElapsedMilliseconds > 600)
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
        grassPlatformAudioClips = new string[5];
        for (int i = 1; i <= grassPlatformAudioClips.Length; i++)
        {
            grassPlatformAudioClips[i - 1] = "Grass0" + i;
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
                    audioClip = snowPlatformAudioClip;
                    break;
                case Utilities.PlatformType.WOOD:
                    audioClip = woodPlatformAudioClip;
                    break;
                case Utilities.PlatformType.MUD:
                    audioClip = mudPlatformAudioClip;
                    break;
                case Utilities.PlatformType.MYSTERIOUS:
                    audioClip = mysteriousPlatformAudioClip;
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
        if (CurBackgroundAmbiance != backgroundAmbiance)
        {
            string oldAmbianceName = curAmbianceName;
            CurBackgroundAmbiance = backgroundAmbiance;
            SetAmbianceName();

            FadeBetweenBackgrounds(oldAmbianceName, curAmbianceName);
        }
    }

    public void PlayBackgroundSong()
    {
        SetSongName();

        if (curSongName != "")
        {
            audioManager.PlaySound(curSongName);
        }
    }

    public void SetBackgroundSong(Utilities.Song song)
    {
        if (CurSong != song)
        {
            string oldSongName = curSongName;
            CurSong = song;
            SetSongName();

            if (curSongName == "")
            {
                audioManager.FadeSound(oldSongName, 0, 0.07f);
            }
            else if (oldSongName == "")
            {
                audioManager.PlaySound(curSongName, 0.75f);
            }
            else
            {
                FadeBetweenBackgrounds(oldSongName, curSongName);
            }
        }
    }

    void SetSongName()
    {
        switch (CurSong)
        {
            case Utilities.Song.LEVEL01:
                curSongName = level01Song;
                break;
            default:
                curSongName = "";
                break;
        }
    }

    /// <summary>
    /// Fades between oldAmbianceName out and curAmbianceName in
    /// </summary>
    /// <param name="oldAmbianceName">The ambiance to be faded out</param>
    void FadeBetweenBackgrounds(string oldSoundName, string newSoundName)
    {
        audioManager.FadeBetweenTwoSounds(newSoundName, oldSoundName);
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
            case Utilities.Ambiance.CAVE:
                curAmbianceName = caveAmbiance;
                break;
            case Utilities.Ambiance.MYSTERIOUS:
                curAmbianceName = mysteriousAmbiance;
                break;
            default:
                UnityEngine.Debug.Log("A non-existent ambiance enum was selected in GM.");
                break;
        }
    }
}