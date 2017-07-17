using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

    public GameObject endLevelUI;

    AudioManager audioManager;
    public string[] forestBackgroundArray;
    string[] woodCrackClips;
    int backgroundSoundIndex;

    void Start () {
        audioManager = AudioManager.instance;
        FindObjectOfType<Controller2D>().hitBranchEvent += OnHitBranch;
        FindObjectOfType<Controller2D>().branchBreakEvent += OnBranchBreak;
        FindObjectOfType<CampFire>().levelEndEvent += OnLevelEnd;
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
        StartCoroutine(LevelEnd());
        FindObjectOfType<CampFire>().levelEndEvent -= OnLevelEnd;
    }

    IEnumerator LevelEnd()
    {
        yield return new WaitForSeconds(2);
        endLevelUI.SetActive(true);
        //loader.SavePlayerStats();
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Level03");
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