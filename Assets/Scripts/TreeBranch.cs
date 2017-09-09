using System.Collections;
using UnityEngine;

public class TreeBranch : MonoBehaviour {

    Controller2D controller2D;
    Transform rotationOrigin;

    public float easeAmount;
    public float rotationSpeed = 2;
    public float rotateAmt = 2;

    void Start()
    {
        controller2D = FindObjectOfType<Controller2D>();
        controller2D.hitBranchEvent += OnHitBranch;
        controller2D.notGroundedEvent += OnOffBranch;
        rotationOrigin = transform.parent;
        //audioManager = AudioManager.instance;

        //woodCrackClips = new string[7];
        //for (int i = 0; i < woodCrackClips.Length; i++)
        //{
        //    woodCrackClips[i] = "woodcrack0" + (i + 1);
        //}
    }

    public virtual void OnHitBranch()
    {
        StartCoroutine(RotateDown(0.5f));
        controller2D.hitBranchEvent -= OnHitBranch;
        controller2D.notGroundedEvent += OnOffBranch;
    }

    public void OnOffBranch()
    {
        StartCoroutine(RotateDown(-0.5f));
        controller2D.hitBranchEvent += OnHitBranch;
        controller2D.notGroundedEvent -= OnOffBranch;
    }

    IEnumerator RotateDown(float rotation)
    {
        float deltaRotation = 0;

        while (Mathf.Abs(deltaRotation) < rotateAmt)
        {
            rotationOrigin.Rotate(new Vector3(0, 0, rotation));
            deltaRotation += rotation;
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
