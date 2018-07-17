using System.Collections;
using UnityEngine;

/// <summary>
/// Children require a Rigidbody2D and SpriteRenderer
/// </summary>

public class ObjectExplode : MonoBehaviour
{

    public float maxThrust = 10;
    public float minThrust = 1;
    public Vector2 xMaxMin;
    public Vector2 yMaxMin;
    public float waitTime = 2f;
    float targetTime = 0;
    Rigidbody2D[] children;
    float speed = 0.03f;

    void Start()
    {
        targetTime = Time.time + waitTime;

        children = GetComponentsInChildren<Rigidbody2D>();

        Debug.Log(children.Length);
        for (int i = 0; i < children.Length; i++)
        {
            float xForce = Random.Range(xMaxMin.x, xMaxMin.y);
            float yForce = Random.Range(yMaxMin.x, yMaxMin.y);
            float thrust = Random.Range(minThrust, maxThrust);

            children[i].AddForce(new Vector2(xForce, yForce) * thrust);
            StartCoroutine(waitToFade(i));
        }
    }

    IEnumerator waitToFade(int index)
    {
        while (Time.time < targetTime)
        {
            yield return null;
        }
        Utilities.instance.FadeObjectOut(children[index].gameObject, speed, false, false);
    }
}
