using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteMask))]
public class MaskFadeIn : MonoBehaviour {

    float startFade = 1f;
    public float endFade = 0.1f;
    SpriteMask mask;

	void Start () {
        mask = GetComponent<SpriteMask>();
        mask.alphaCutoff = startFade;
        StartCoroutine(FadeIn());
	}

    IEnumerator FadeIn()
    {
        while (mask.alphaCutoff > endFade)
        {
            mask.alphaCutoff = Mathf.Lerp(mask.alphaCutoff, mask.alphaCutoff - 0.1f, 1f);
            yield return null;
        }
    }
}
