using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhino : MonoBehaviour {

    Player player;
    public Transform head;
    Animator anim;
    public float easeAmount;
    public float rotateUpTarget = -10f;
    public float rotateDownTarget = 10;
    public float rotationSpeed = 2;
    public float maxMoveSpeed = 1;
    public float accelerationTime = .2f;
    public float maxJumpHeight;
    public float timeToJumpApex = .4f;
    public int chargeDirection = -1;
    float gravity;

    float velocityXSmoothing;
    float moveSpeed = 0;

    RhinoController controller;
    Vector2 velocity;

    public delegate void OnSeePlayer();
    public event OnSeePlayer seePlayerEvent;

    // Use this for initialization
    void Start() {
        player = FindObjectOfType<Player>();
        anim = GetComponent<Animator>();
        seePlayerEvent += SeePlayer;
        controller = transform.parent.GetComponent<RhinoController>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    }

    // Update is called once per frame
    void Update() {
        //Debug.Log(head.rotation.z * Mathf.Rad2Deg);
        //Debug.Log(Mathf.Abs(player.transform.position.x - transform.position.x));
        if (Mathf.Abs(player.transform.position.x - transform.position.x) < 20)
        {
            if (seePlayerEvent != null)
            {
                seePlayerEvent();
            }
        }

        // Velocity on y axis reset if collision above or below player
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
        CalculateVelocity();
        controller.Move(velocity, false);
    }

    void SeePlayer()
    {
        StartCoroutine(RotateHeadUp());
        seePlayerEvent -= SeePlayer;
    }

    IEnumerator RotateHeadUp()
    {
        anim.enabled = false;
        float totalRotation = Mathf.Abs(rotateUpTarget - head.rotation.z * Mathf.Rad2Deg);
        float currentRotation;
        float rotationPercentage;

        while (head.rotation.z * Mathf.Rad2Deg > rotateUpTarget + 2)
        {
            currentRotation = Mathf.Abs(rotateUpTarget - head.rotation.z * Mathf.Rad2Deg);
            rotationPercentage = currentRotation / totalRotation;
            rotationPercentage = Mathf.Clamp01(rotationPercentage);
            float easedPercentBetweenRotation = -Ease(rotationPercentage);
            Debug.Log(head.rotation.z * Mathf.Rad2Deg + " " + (rotateUpTarget + 2));
            head.Rotate(Vector3.forward, easedPercentBetweenRotation * rotationSpeed * Time.deltaTime);

            yield return null;
        }
        yield return new WaitForSeconds(.1f);
        StartCoroutine(RotateHeadDown());
    }

    IEnumerator RotateHeadDown()
    {
        anim.enabled = false;
        float totalRotation = Mathf.Abs(rotateDownTarget - head.rotation.z * Mathf.Rad2Deg);
        float currentRotation;
        float rotationPercentage;

        while (head.rotation.z * Mathf.Rad2Deg < rotateDownTarget - 2)
        {
            currentRotation = Mathf.Abs(rotateDownTarget - head.rotation.z * Mathf.Rad2Deg);
            rotationPercentage = currentRotation / totalRotation;
            rotationPercentage = Mathf.Clamp01(rotationPercentage);
            float easedPercentBetweenRotation = Ease(rotationPercentage);
            Debug.Log(head.rotation.z * Mathf.Rad2Deg + "  target: " + (rotateDownTarget - 2));
            head.Rotate(Vector3.forward, easedPercentBetweenRotation * rotationSpeed * Time.deltaTime);

            yield return null;
        }
        yield return new WaitForSeconds(.1f);
        Charge();
    }

    void Charge()
    {
        anim.enabled = true;
        anim.Play("Run");
        StartCoroutine(Charging());
    }

    IEnumerator Charging()
    {
        moveSpeed = 0.2f;
        while (true)
        {
            if(Mathf.Abs(moveSpeed) < maxMoveSpeed)
            {
                moveSpeed = moveSpeed + 0.02f;
            }
            yield return null;
        }
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    void CalculateVelocity()
    {

        float targetVelocityX = moveSpeed * Mathf.Sign(chargeDirection);
        velocity.y += gravity * Time.deltaTime;
        Debug.Log(velocity.y + " " + moveSpeed);
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);
    }
}
