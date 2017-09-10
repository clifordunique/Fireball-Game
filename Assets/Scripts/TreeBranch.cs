using System.Collections;
using UnityEngine;

public class TreeBranch : MonoBehaviour {

    Controller2D controller2D;
    Transform rotationOrigin;
    Rigidbody2D rb2d;

    public float easeAmount;
    public float rotationSpeed = 2;
    public float rotateAmt = 2;

    float originalRotation;
    bool RDIsRunning;
    bool RUIsRunning;

    void Start()
    {
        controller2D = FindObjectOfType<Controller2D>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        rotationOrigin = transform.parent;
        originalRotation = transform.rotation.z;
        
        //audioManager = AudioManager.instance;

        //woodCrackClips = new string[7];
        //for (int i = 0; i < woodCrackClips.Length; i++)
        //{
        //    woodCrackClips[i] = "woodcrack0" + (i + 1);
        //}
    }

    public virtual void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Enter");
        if(col.gameObject.tag == "Player")
        {
            StartCoroutine(RotateDown(rotationSpeed));
        }
    }

    public virtual void OnCollisionExit2D(Collision2D col)
    {
        Debug.Log("Exit");
        if (col.gameObject.tag == "Player")
        {
            StartCoroutine(RotateUp(-rotationSpeed));
        }
    }

    //public virtual void OnHitBranch()
    //{
    //    Debug.Log("hit");
    //    StartCoroutine(Rotate(0.5f));
    //    controller2D.hitBranchEvent -= OnHitBranch;
    //}

    IEnumerator RotateDown(float rotation)
    {
        RDIsRunning = true;
        float deltaRotation = 0;
        float rotationPercentage;
        while (deltaRotation < rotateAmt)
        {
            deltaRotation += rotation;
            rotationPercentage = deltaRotation / rotateAmt;
            rotationPercentage = Mathf.Clamp01(rotationPercentage);
            float easedPercentBetweenRotation = Ease(rotationPercentage);
            //rotationOrigin.Rotate(new Vector3(0, 0, rotation));
            rotationOrigin.Rotate(Vector3.forward, easedPercentBetweenRotation * rotation * Time.deltaTime);
            yield return null;
        }
        RDIsRunning = false;
    }

    IEnumerator RotateUp(float rotation)
    {
        RUIsRunning = true;
        float deltaRotation = 0;
        float rotationPercentage;

        while (deltaRotation > -rotateAmt)
        {
            Debug.Log(deltaRotation);
            deltaRotation += rotation;
            rotationPercentage = Mathf.Abs(deltaRotation / rotateAmt);
            rotationPercentage = Mathf.Clamp01(rotationPercentage);
            Debug.Log(rotationPercentage);
            float easedPercentBetweenRotation = Ease(rotationPercentage);
            //rotationOrigin.Rotate(new Vector3(0, 0, rotation));
            rotationOrigin.Rotate(Vector3.forward, easedPercentBetweenRotation * rotation * Time.deltaTime);
            yield return null;
        }
        RUIsRunning = false;
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
