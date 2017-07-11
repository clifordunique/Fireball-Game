using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

    public GameObject endLevelUI;

    AudioManager audioManager;
    public string[] forestBackgroundArray;
    string[] woodCrackClips;
    int backgroundSoundIndex;

    float maxHealth;
    float fireHealth;
    float maxFireHealth;
    float moveSpeed;
    float maxJumpHeight;

    bool isFire;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

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
        SavePlayerStats();
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

    /* saves the player stats, such as powerups and if he's still on fire
     */
    public void SavePlayerStats()
    {
        Player player = FindObjectOfType<Player>();

        //maxHealth = player.maxHealth;
        //maxFireHealth = player.maxFireHealth;
        fireHealth = player.fireHealth;
        //moveSpeed = player.moveSpeed;
        //maxJumpHeight = player.maxJumpHeight;
        isFire = player.isFire;
    }

    /* loads the player stats, such as powerups and if he's still on fire
 */
    public void LoadPlayerStats()
    {
        Player player = FindObjectOfType<Player>();

        //player.maxHealth  = maxHealth;
        //player.maxFireHealth  = maxFireHealth;
        player.fireHealth = fireHealth;
        //player.moveSpeed  = moveSpeed;
        //player.maxJumpHeight  = maxJumpHeight;
        player.isFire  = isFire;
    }

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