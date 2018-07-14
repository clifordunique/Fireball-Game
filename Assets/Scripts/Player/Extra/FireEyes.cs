using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEyes : MonoBehaviour
{

    public SpriteRenderer fireEyes1, fireEyes2;
    // Should be between 0 and 1, and oddly enough, the lower the speed, the slower the flicker.
    public Vector2 flickerSpeedMaxMin = new Vector2(0.2f, 1f);
    public Vector2 flickerAmountMaxMin = new Vector2(0.7f, 0.9f);
    public Vector2 waitBtwFlickerMaxMin = new Vector2(0.01f, 2f);

    public float fireBase = 1f;

    private void Start()
    {
        StartCoroutine(Flicker(fireEyes1));
        StartCoroutine(Flicker(fireEyes2));
    }

    /// <summary>
    /// Sets the base apha percentage for the Golem's eyes.
    /// </summary>
    /// <param name="_fireHealth">The current fire health</param>
    /// <param name="_maxFireHealth">The max fire health</param>
    public void SetFireBase(int _fireBase, int _maxFireHealth)
    {
        fireBase = (float)_fireBase / (float)_maxFireHealth;
    }

    /// <summary>
    /// Animated the object by making it flicker darker and then back to its previous alpha.
    /// </summary>
    /// <param name="fireEyes">The SpriteRenderer to apply a flicker effect to</param>
    /// <returns>null</returns>
    IEnumerator Flicker(SpriteRenderer fireEyes)
    {
        //StartCoroutine(FlickerIndividual(fireEyes));
        while (true)
        {

            float flickerSpeed = Random.Range(flickerSpeedMaxMin.x, flickerSpeedMaxMin.y);
            float flickerAmount = Random.Range(flickerAmountMaxMin.x, flickerAmountMaxMin.y);


            while (fireEyes.color.a > fireBase * flickerAmount + 0.01f)
            {
                fireEyes.color = new Color(fireEyes.color.r, fireEyes.color.g, fireEyes.color.b, Mathf.Lerp(fireEyes.color.a, fireBase * flickerAmount, flickerSpeed));
                yield return null;
            }
            while (fireEyes.color.a < fireBase - 0.01f)
            {
                fireEyes.color = new Color(fireEyes.color.r, fireEyes.color.g, fireEyes.color.b, Mathf.Lerp(fireEyes.color.a, fireBase, flickerSpeed));
                yield return null;
            }

            yield return FlickerIndividual(fireEyes);
        }
    }

    IEnumerator FlickerIndividual(SpriteRenderer fireEyes)
    {
        float waitBtwFlicker = Random.Range(waitBtwFlickerMaxMin.x, waitBtwFlickerMaxMin.y);
        float targetTime = Time.time + waitBtwFlicker;
        while (Time.time < targetTime)
        {
            fireEyes.color = new Color(fireEyes.color.r, fireEyes.color.g, fireEyes.color.b, fireBase);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
