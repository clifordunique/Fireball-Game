using System.Collections;
using UnityEngine;

public class TreeBranch : MonoBehaviour
{
    // There might be more than one of this script on an object. This bool detects if the player has already collided
    // with one so that, for example, if this object has already rotated down, it won't rotate down again.
    public static bool collided;

    public Transform rotationOrigin;
    public Transform[] children;
    Rigidbody2D rb2d;

    public float easeAmount;
    public float rotationSpeed = 2;
    public float rotateAmt = 2;
    public float childRotateAmt = 10;
    public bool leftSideOfTree;

    float originalRotation;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        originalRotation = transform.rotation.z;
        if (!leftSideOfTree)
        {
            rotationSpeed = -rotationSpeed;
        }

        //audioManager = AudioManager.instance;

        //woodCrackClips = new string[7];
        //for (int i = 0; i < woodCrackClips.Length; i++)
        //{
        //    woodCrackClips[i] = "woodcrack0" + (i + 1);
        //}
    }

    public virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (collided == false)
        {
            if (col.gameObject.tag == "Player")
            {
                StartCoroutine(Rotate(rotationSpeed, false));
                if (children.Length > 0)
                {
                    StartCoroutine(RotateChildrenStart());
                }
            }
        }

        collided = true;
    }

    public virtual void OnCollisionExit2D(Collision2D col)
    {
        if (collided == true)
        {
            if (col.gameObject.tag == "Player")
            {
                StartCoroutine(Rotate(-rotationSpeed, true));
                if (children.Length > 0)
                {
                    StartCoroutine(RotateChildrenStart());
                }
            }
        }
        collided = false;
        //if(collided == true){
        //	rotated = true
        //}
    }

    /* Rotates rotationOrigin a certain amount
     * Note: this should be used to rotate a sprite and not a collider.
     * Strange things might happen otherwise.
     * 
     * @param rotation - the amount to rotate
     */
    IEnumerator Rotate(float rotation, bool rotateBack)
    {
        float deltaRotation = 0;
        float rotationPercentage;
        while (deltaRotation < rotateAmt)
        {
            deltaRotation += Mathf.Abs(rotation);
            rotationPercentage = Mathf.Abs(deltaRotation / rotateAmt);
            rotationPercentage = Mathf.Clamp01(rotationPercentage);
            float easedPercentBetweenRotation = Ease(rotationPercentage);
            rotationOrigin.Rotate(Vector3.forward, easedPercentBetweenRotation * rotation * Time.deltaTime);
            yield return null;
        }
        //if (rotateBack)
        //{
        //    Debug.Log("Back up");
        //    StartCoroutine(Rotate(-2 * rotationSpeed, false));
        //}
    }

    IEnumerator RotateChildrenStart()
    {

        for (int i = 0; i < children.Length; i++)
        {
            StartCoroutine(RotateChildren(children[i], i % 2 == 2 ? -1:1));
            yield return null;
        }
    }

    IEnumerator RotateChildren(Transform child, int direction)
    {
        float deltaRotation = 0;
        while (deltaRotation < childRotateAmt)
        {
            child.Rotate(new Vector3(0, 0, .1f * direction));
            deltaRotation += .1f * direction;
            yield return null;
        }
        while (deltaRotation > 0)
        {
            //Debug.Log("deltaRotation " + deltaRotation + " rotateAmt " + (rotateAmt));
            child.Rotate(new Vector3(0, 0, -.1f * direction));
            deltaRotation -= .1f * direction;
            yield return null;
        }
    }

    //IEnumerator RotateUp(float rotation)
    //{
    //    float deltaRotation = 0;
    //    float rotationPercentage;

    //    while (deltaRotation > -rotateAmt)
    //    {
    //        deltaRotation += rotation;
    //        rotationPercentage = Mathf.Abs(deltaRotation / rotateAmt);
    //        rotationPercentage = Mathf.Clamp01(rotationPercentage);
    //        float easedPercentBetweenRotation = Ease(rotationPercentage);
    //        rotationOrigin.Rotate(Vector3.forward, easedPercentBetweenRotation * rotation * Time.deltaTime);
    //        yield return null;
    //    }
    //}

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
}
