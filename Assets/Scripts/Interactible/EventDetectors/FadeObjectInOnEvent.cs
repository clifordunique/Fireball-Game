using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeObjectInOnEvent : MonoBehaviour
{
    public float waitTime = 2f;
    public GameObject[] textObjects;

    private void Start()
    {
        Enemy.onEnemyDestroy += EnemyIsDestroyed;
    }

    private void EnemyIsDestroyed(GameObject gameObject)
    {
        StartCoroutine(TextHandler());
        Enemy.onEnemyDestroy -= EnemyIsDestroyed;
    }

    IEnumerator TextHandler()
    {
        for (int i = 0; i < textObjects.Length; i++)
        {
            textObjects[i].gameObject.SetActive(true);

            Utilities.instance.FadeObjectIn(textObjects[i], 0.08f);

            yield return new WaitForSeconds(waitTime);

            Utilities.instance.FadeObjectOut(textObjects[i], 0.08f, false, true);
        }
    }
}
