using System.Collections;
using UnityEngine;
using System.Diagnostics;

public class TreeBranch : MonoBehaviour
{
    public static bool collided;

    Controller2D controller2D;
    Transform rotationOrigin;
    Rigidbody2D rb2d;
    Stopwatch sw;

    public float easeAmount;
    public float rotationSpeed = 2;
    public float rotateAmt = 2;
    public float waitMilliseconds;

    float originalRotation;

    void Start()
    {
        controller2D = FindObjectOfType<Controller2D>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        rotationOrigin = transform.parent;
        originalRotation = transform.rotation.z;
        sw = new Stopwatch();
        sw.Start();
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
                StartCoroutine(RotateDown(rotationSpeed, col.transform));
            }
        }

        collided = true;
    }

    //public virtual void OnCollisionStay2D(Collision2D col)
    //{
    //    collided = true;
    //    if (sw.ElapsedMilliseconds > waitMilliseconds)
    //    {
    //        if (col.gameObject.tag == "Player")
    //        {
    //            StartCoroutine(RotateDown(rotationSpeed));
    //        }
    //        sw.Reset();
    //        sw.Start();
    //    }
    //}

    public virtual void OnCollisionExit2D(Collision2D col)
    {
        collided = false;
        if (col.gameObject.tag == "Player")
        {
            StartCoroutine(RotateUp(-rotationSpeed, col.transform));
        }
    }

    //public virtual void OnHitBranch()
    //{
    //    Debug.Log("hit");
    //    StartCoroutine(Rotate(0.5f));
    //    controller2D.hitBranchEvent -= OnHitBranch;
    //}

    IEnumerator RotateDown(float rotation, Transform player)
    {
        float deltaRotation = 0;
        float rotationPercentage;
        while (deltaRotation < rotateAmt)
        {
            deltaRotation += rotation;
            rotationPercentage = deltaRotation / rotateAmt;
            rotationPercentage = Mathf.Clamp01(rotationPercentage);
            float easedPercentBetweenRotation = Ease(rotationPercentage);
            rotationOrigin.Rotate(Vector3.forward, easedPercentBetweenRotation * rotation * Time.deltaTime);
            player.RotateAround(rotationOrigin.position, Vector3.forward, easedPercentBetweenRotation * rotation * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator RotateUp(float rotation, Transform player)
    {
        float deltaRotation = 0;
        float rotationPercentage;

        while (deltaRotation > -rotateAmt)
        {
            deltaRotation += rotation;
            rotationPercentage = Mathf.Abs(deltaRotation / rotateAmt);
            rotationPercentage = Mathf.Clamp01(rotationPercentage);
            float easedPercentBetweenRotation = Ease(rotationPercentage);
            rotationOrigin.Rotate(Vector3.forward, easedPercentBetweenRotation * rotation * Time.deltaTime);
            player.RotateAround(rotationOrigin.position, Vector3.forward, easedPercentBetweenRotation * rotation * Time.deltaTime);
            yield return null;
        }
    }

    //IEnumerator RotateDown()
    //{
    //    float totalRotation = Mathf.Abs(rotateDownTarget + rotationOrigin.rotation.z * Mathf.Rad2Deg);
    //    float currentRotation;
    //    float rotationPercentage;

    //    Debug.Log(rotateDownTarget);

    //    while (rotationOrigin.rotation.z * Mathf.Rad2Deg < rotateDownTarget)
    //    {
    //        currentRotation = Mathf.Abs(rotateDownTarget - rotationOrigin.rotation.z * Mathf.Rad2Deg);
    //        rotationPercentage = currentRotation / totalRotation;
    //        rotationPercentage = Mathf.Clamp01(rotationPercentage);
    //        float easedPercentBetweenRotation = Ease(rotationPercentage);
    //        rotationOrigin.Rotate(Vector3.forward, easedPercentBetweenRotation * rotationSpeed * Time.deltaTime);

    //        yield return null;
    //    }
    //}

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
}
