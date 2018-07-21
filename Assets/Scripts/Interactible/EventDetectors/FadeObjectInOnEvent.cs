﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        StartCoroutine(TextHandler());
        Enemy.onEnemyDestroy -= EnemyIsDestroyed;
    }

    IEnumerator TextHandler()
    {
        for (int i = 0; i < textObjects.Length; i++)
        {
            textObjects[i].gameObject.SetActive(true);
            Color color1 = textObjects[i].gameObject.GetComponent<Image>().color;
            textObjects[i].gameObject.GetComponent<Image>().color = new Color(color1.r, color1.g, color1.b, 0);
            Utilities.instance.FadeObjectIn(textObjects[i], 0.08f);

            Color color2 = textObjects[i].gameObject.GetComponentInChildren<Text>().color;
            textObjects[i].gameObject.GetComponent<Image>().color = new Color(color2.r, color2.g, color2.b, 0);
            Utilities.instance.FadeObjectIn(textObjects[i].GetComponentInChildren<Text>().gameObject, 0.08f);

            yield return new WaitForSeconds(waitTime);

            Utilities.instance.FadeObjectOut(textObjects[i], 0.08f, false, true);
            Utilities.instance.FadeObjectOut(textObjects[i].GetComponentInChildren<Text>().gameObject, 0.08f, false, true);
        }
    }
}
