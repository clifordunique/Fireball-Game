using System.Collections;
using UnityEngine;

public class TSEDFade : TSEnableDisableObject
{
    public float min, max;

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            SpriteMask temp = actionObject.GetComponent<SpriteMask>();
            base.OnTriggerEnter2D(col);
            StopAllCoroutines();
            StartCoroutine(Fade(temp, min, true));
        }
    }

    public override void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            SpriteMask temp = actionObject.GetComponent<SpriteMask>();
            StopAllCoroutines();
            StartCoroutine(Fade(temp, max, false));
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
