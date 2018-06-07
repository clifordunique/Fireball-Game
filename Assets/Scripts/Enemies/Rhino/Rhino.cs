using System.Collections;
using UnityEngine;

public class Rhino : Enemy {

    public LayerMask layerMask;
    public Transform head;
    public Transform eye;
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

    Player player;
    Animator anim;
    AudioManager audioManager;
    AudioSource rhinoRun;
    RhinoController controller;
    Vector2 velocity;

    public delegate void OnSeePlayer();
    public event OnSeePlayer seePlayerEvent;

    // Use this for initialization
    public override void Start() {
        base.Start();
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
        }
        rhinoRun = GetComponent<AudioSource>();
        player = FindObjectOfType<Player>();
        anim = GetComponent<Animator>();
        seePlayerEvent += SeePlayer;
        controller = transform.parent.GetComponent<RhinoController>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    }

    // Update is called once per frame
    void Update()
    {
        // First check and see if the state is paused. If it is, return.
        if (GameMaster.gm.CurState == Utilities.State.PAUSED) return;

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
        CanSeePlayer();
    }

    void CanSeePlayer()
    {
        if(player != null)
        {
            Vector2 directionToPlayer = player.transform.position - eye.position;
            RaycastHit2D hit = Physics2D.Raycast(eye.position, directionToPlayer, seePlayerDistanceX, layerMask);
            Debug.DrawRay(eye.position, directionToPlayer * seePlayerDistanceX, Color.red);
            if (hit && hit.transform.gameObject.tag == "Player" && hit.transform.gameObject.GetComponent<SpriteRenderer>().sortingLayerName == "Player")
            {
                if (seePlayerEvent != null)
                {
                    seePlayerEvent();
                }
            }
        }
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

        while (head.rotation.z * Mathf.Rad2Deg < rotateDownTarget - 3)
        {
            currentRotation = Mathf.Abs(rotateDownTarget - head.rotation.z * Mathf.Rad2Deg);
            rotationPercentage = currentRotation / totalRotation;
            rotationPercentage = Mathf.Clamp01(rotationPercentage);
            float easedPercentBetweenRotation = Ease(rotationPercentage);
            //Debug.Log(head.rotation.z * Mathf.Rad2Deg + "  target: " + (rotateDownTarget - 2));
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
        rhinoRun.Play();
    }

    IEnumerator Charging()
    {
        moveSpeed = 0.2f;
        while (true)
        { 
            if (Mathf.Abs(moveSpeed) < maxMoveSpeed)
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

    /* Calculates x and y velocity
     */
    void CalculateVelocity()
    {
        float targetVelocityX = moveSpeed * Mathf.Sign(chargeDirection);
        velocity.y += gravity * Time.deltaTime;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);
    }
}
