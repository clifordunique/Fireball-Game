using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

    public GameObject endLevelUI;

    AudioManager audioManager;
    public string[] forestBackgroundArray;
    string[] woodCrackClips;
    int backgroundSoundIndex;

    void Awake()
    {
        if(FindObjectOfType<Player>() != null)
        {
            FindObjectOfType<Player>().fingPoop += Crap;
        }
        if(FindObjectOfType<Controller2D>() != null)
        {
            FindObjectOfType<Controller2D>().hitBranchEvent += OnHitBranch;
            FindObjectOfType<Controller2D>().branchBreakEvent += OnBranchBreak;
        }
        if(FindObjectOfType<CampFire>() != null)
        {
            FindObjectOfType<CampFire>().levelEndEvent += OnLevelEnd;
        }
    }

    void Start ()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
        }
        woodCrackClips = new string[7];
        for (int i = 0; i < woodCrackClips.Length; i++)
        {
            woodCrackClips[i] = "woodcrack0" + (i + 1);
        }
        GetRandomIndex();
        PlayBackgroundMusic();
    }

    // Restarts the level
    void Crap()
    {
        StartCoroutine(FadeOut(2));
    }

    void LateUpdate()
    {
        if(!audioManager.isPlaying(forestBackgroundArray[backgroundSoundIndex]))
        {
            GetRandomIndex();
            PlayBackgroundMusic();
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

    public void OnRestartLevel()
    {

    }

    IEnumerator FadeOut(int action)
    {
        yield return new WaitForSeconds(2);
        endLevelUI.SetActive(true);
        yield return new WaitForSeconds(2);
        switch(action)
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

    public void OnHitBranch()
    {
        int i = Random.Range(1, woodCrackClips.Length);
        Debug.Log("poop " + woodCrackClips[i]);
        audioManager.PlaySound(woodCrackClips[i]);
    }
    public void OnBranchBreak()
    {
        audioManager.PlaySound(woodCrackClips[0]);
        new WaitForSeconds(1f);
        audioManager.PlaySound(woodCrackClips[5]);
        FindObjectOfType<Controller2D>().hitBranchEvent -= OnHitBranch;
        FindObjectOfType<Controller2D>().branchBreakEvent -= OnBranchBreak;
    }
}