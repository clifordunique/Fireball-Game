using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMask : MonoBehaviour
{
    SpriteMask mask;

    // Use this for initialization
    void Start()
    {
        mask = GetComponent<SpriteMask>();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (mask.alphaCutoff > 0)
        {
            mask.alphaCutoff = Mathf.Lerp(mask.alphaCutoff, mask.alphaCutoff - 0.02f, 1f);
            yield return new WaitForSeconds(0.005f);
        }
    }

}
