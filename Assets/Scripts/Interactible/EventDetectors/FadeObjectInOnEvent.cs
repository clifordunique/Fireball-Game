using System.Collections;
using UnityEngine;

public class FadeObjectInOnEvent : MonoBehaviour
{
    public Enemy enemy;
    public float waitTime = 2f;
    public GameObject[] textObjects;

    private void Start()
    {
        Enemy.onEnemyDestroy += EnemyIsDestroyed;
    }

    private void EnemyIsDestroyed()
    {
        Debug.Log("Enemey is tdestry");
        StartCoroutine(TextHandler());
        Enemy.onEnemyDestroy -= EnemyIsDestroyed;
    }

    IEnumerator TextHandler()
    {
        Debug.Log("coroufi");
        for (int i = 0; i < textObjects.Length; i++)
        {
            textObjects[i].gameObject.SetActive(true);
            Utilities.instance.FadeObjectIn(textObjects[i], 0.03f);
            yield return new WaitForSeconds(waitTime);
            Utilities.instance.FadeObjectOut(textObjects[i], 0.03f, false, true);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
