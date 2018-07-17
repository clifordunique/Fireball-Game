using System.Collections;
using UnityEngine;

public class ObjectDestroy : MonoBehaviour
{
    public float maxThrust, minThrust;
    public bool fadeOut;

    SpriteRenderer sr;
    Rigidbody2D rb2D;
    float yForce;
    float xForce;
    float thrust;
    AudioManager audioManager;

    void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
        }
        sr = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        yForce = Random.Range(-0.5f, 2);
        xForce = Random.Range(-1, 0);
        thrust = Random.Range(minThrust, maxThrust);
        rb2D.AddForce(new Vector2(xForce, yForce) * thrust);
        if (fadeOut)
        {
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        Color tmp = sr.color;
        while (sr.color.a >= 0)
        {
            tmp.a -= 0.02f;
            sr.color = tmp;
            yield return null;
        }
    }

    /* If colliding with an enemy or plant, ignore the collision
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Underbrush"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
        }
    }
}
