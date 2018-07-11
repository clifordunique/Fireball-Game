using System.Collections;
using UnityEngine;

public class TSEDFade : TSEnableDisableObject
{
    public float min, max;
    public string soundToFade;
    public float volumePercentageFade = 0.1f;
    public float fadeSpeed = 0.01f;

    AudioManager am;
    float originalVolume;

    private void Start()
    {
        am = AudioManager.instance;
        originalVolume = am.GetVolume(soundToFade);
    }

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            SpriteMask temp = actionObject.GetComponent<SpriteMask>();
            base.OnTriggerEnter2D(col);
            StopAllCoroutines();
            StartCoroutine(Fade(temp, min, true));
            am.FadeSound(soundToFade, volumePercentageFade, fadeSpeed);
        }
    }

    public override void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            SpriteMask temp = actionObject.GetComponent<SpriteMask>();
            StopAllCoroutines();
            StartCoroutine(Fade(temp, max, false));
            am.FadeSound(soundToFade, originalVolume, fadeSpeed);
        }
    }


    IEnumerator Fade(SpriteMask mask, float end, bool isActive)
    {
        if (mask.alphaCutoff > end)
        {
            while (mask.alphaCutoff > end)
            {
                mask.alphaCutoff = Mathf.Lerp(mask.alphaCutoff, mask.alphaCutoff - 0.02f, 1f);
                yield return null;
            }
        }
        else
        {
            while (mask.alphaCutoff < end)
            {
                mask.alphaCutoff = Mathf.Lerp(mask.alphaCutoff, mask.alphaCutoff + 0.02f, 1f);
                yield return null;
            }
        }

        if (!isActive)
        {
            mask.gameObject.SetActive(false);
        }
    }
}
