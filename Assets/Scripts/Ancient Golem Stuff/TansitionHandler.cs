using System.Collections;
using UnityEngine;

public class TansitionHandler : MonoBehaviour
{

    public Utilities.Song TransitionToSong;
    public Utilities.Song TransitionBackSong;
    public Utilities.Ambiance TransitionToAmbiance;
    public Utilities.Ambiance TranstionBackAmbiance;

    public GameObject enterShineAnimation;
    public GameObject exitShineAnimation;
    public GameObject ancientGolem;
    public Player player;
    public Transform transformToPos;

    public float waitTime = 2f;

    Vector2 savedPlayerPos;
    GameMaster gameMaster;
    Animator animator;

    // Use this for initialization
    void Start()
    {
        LightOnFire.onFire += TransitionToMysterious;
        gameMaster = GameMaster.gm;
        animator = GetComponent<Animator>();
    }

    void TransitionToMysterious()
    {
        savedPlayerPos = player.transform.position;
        StartCoroutine(TransportThereAction());

        LightOnFire.onFire -= TransitionToMysterious;
    }

    void TransitionBack()
    {
        gameMaster.SetAmbianceEnum(TranstionBackAmbiance);
        gameMaster.SetBackgroundSong(TransitionBackSong);
        exitShineAnimation.GetComponent<Animator>().enabled = true;
        exitShineAnimation.GetComponent<Animator>().Play("ShineThingExit");

        GameMaster.gm.GetComponent<CameraShake>().Shake(0.3f, 2f);
        AudioManager.instance.PlaySound("Shockwave");

        StartCoroutine(TransportBackAction());
    }

    IEnumerator TransportThereAction()
    {
        float targetTime = Time.time + waitTime;

        // wait
        while (Time.time < targetTime)
        {
            yield return new WaitForSeconds(0.05f);
        }

        GameMaster.gm.GetComponent<CameraShake>().Shake(0.3f, 2f);
        AudioManager.instance.PlaySound("Shockwave");
        ancientGolem.GetComponent<Animator>().enabled = true;
        ancientGolem.GetComponent<Animator>().Play("FlashTransition");
        enterShineAnimation.GetComponent<Animator>().enabled = true;

        gameMaster.SetAmbianceEnum(TransitionToAmbiance);
        gameMaster.SetBackgroundSong(TransitionToSong);

        targetTime = Time.time + 1f;
        // wait
        while (Time.time < targetTime)
        {
            yield return new WaitForSeconds(0.05f);
        }
        enterShineAnimation.GetComponent<Animator>().Play("ShineThingEnter");
        player.transform.position = transformToPos.position;
    }

    IEnumerator TransportBackAction()
    {
        float targetTime = Time.time + 1f;

        while (Time.time < targetTime)
        {
            yield return new WaitForSeconds(0.05f);
        }

        player.transform.position = savedPlayerPos;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            TransitionBack();
        }
    }
}
