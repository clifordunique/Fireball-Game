using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

    public GameObject endLevelUI;

    AudioManager audioManager;
    public string[] forestBackgroundArray;
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

        if (audioManager == null)
        {
            Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
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

    public void EndLevel()
    {
        endLevelUI.SetActive(true);
        SavePlayerStats();
        StartCoroutine(waitToLoad(2));
    }

    IEnumerator waitToLoad(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene("Level03");

    }

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
}
