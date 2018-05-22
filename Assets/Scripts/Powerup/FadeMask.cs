using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMask : MonoBehaviour
{
    SpriteMask mask;

    // Use this for initialization
    void Start()
    {
        Debug.Log("poop");
        mask = GetComponent<SpriteMask>();

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (mask.alphaCutoff > 0)
        {
            mask.alphaCutoff = Mathf.Lerp(mask.alphaCutoff, mask.alphaCutoff - 0.03f, 1f);
            yield return null;
        }
    }

}
