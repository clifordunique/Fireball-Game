/* Name: Player.cs
 * Author: Sebastian Lague
 * Modified by John Paul Depew
 * 
 * Description: This script handles a lot of the variables of the Player class.
 * It also handles wall jumping and calculating the velocity.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour, FallInWaterableObject
{
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public LayerMask underBrushLayerMask;
    float accelerationTimeAirborne = .3f;
    float accelerationTimeGrounded = .2f;
    float accelerationTimeDescendingSlope = .5f;
    float accelerationTimeClimbingSlope = .1f;

    public float health;

    // Variables to be carried over to the next scene start
    public float maxHealth;
    public float maxFireHealth;
    public float fireHealth;
    public float moveSpeed = 40;
    public float maxJumpHeight;
    public bool isFire = true;
    // End

    GameMaster gm;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    bool isNearUnderbrush = false;
    bool wantsToBeInUnderBrush = false;
    float timeToWallUnstick;

    float gravity;       // -(2 * maxJumpHeight) / timeToJumpApex^2
    float gravityOriginal;
    float maxJumpVelocity;  // gravity * timeToJumpApex
    float minJumpVelocity;
    public Vector3 velocity;
    float velocityXSmoothing;
    public bool isInWater = false;
    public bool isDoubleJumping;
    PlatformType platformType;

    CameraShake camShake;
    SpriteRenderer sr;
    CollisionInfo colInfo;
    Controller2D controller;
    Animator anim;
    AudioManager audioManager;
    public GameObject deathPrefab;
    string audioClip = null;
    string[] audioClips;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    float velocityXOld;
    float velocityYOld;

    // Power ups
    public bool canDoubleJump;
    bool timeIsOut = false;

    //CameraShake variables
    public float camShakeAmt = 0.1f;
    public float camShakeLength = 0.1f;

    // DELEGATES
    public delegate void OnFireChange(bool currentFire);
    public event OnFireChange onFireChangeEvent;

    public delegate void OnUnderbrush();
    public event OnUnderbrush onUnderbrushEvent;

    void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("fREAK OUT, NO AUDIOMANAGER IN SCENE!!!");
        }
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        gravityOriginal = gravity;
        gm = GameMaster.gm;
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        health = maxHealth;
        anim.SetFloat("Fire Health", fireHealth);
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if (camShake == null)
        {
            Debug.LogError("No CameraShake found on the GameMaster.");
        }
        //print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
        //audioClips = new string[14];
        //for (int i = 0; i < audioClips.Length; i++)
        //{
        //    if (i < 9)
        //        audioClips[i] = "grass" + "0" + (i + 1);
        //    else
        //        audioClips[i] = "grass" + (i + 1);
        //}
    }

    void Update()
    {
        CalculateVelocity();
        HandleWallSliding();
        DetectUnderBrush();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ToggleIsInUnderbrush();
        }
        if (Input.GetButton("Shift"))
        {
            OnShiftInput();
        }
        controller.Move(velocity * Time.deltaTime, directionalInput, isDoubleJumping);

        if (isInWater)
        {
            gravity = .5f * gravityOriginal;
        }
        else
        {
            gravity = gravityOriginal;
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

        DealWithFire();
    }

    void DetectUnderBrush()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1f, underBrushLayerMask);
        if (hit)
        {
            if (hit.collider.CompareTag("Underbrush"))
            {
                isNearUnderbrush = true;
                // This if is so that if the player goes out of the underbrush but quickly goes back in he won't suddenly be in the Player layer
                if (wantsToBeInUnderBrush)
                {
                    sr.sortingLayerName = "Behind Underbrush";
                }
            }
        }
        else
        {
            isNearUnderbrush = false;
            sr.sortingLayerName = "Player";
        }
    }

    /* Toggles if the player is behind the underbrush
    */
    void ToggleIsInUnderbrush()
    {
        if (isNearUnderbrush)
        {
            audioManager.PlaySound("underbrush");
            if (sr.sortingLayerName == "Player")
            {
                sr.sortingLayerName = "Behind Underbrush";
                wantsToBeInUnderBrush = true;
                if (onUnderbrushEvent != null)
                {
                    onUnderbrushEvent();
                }

            }
            else
            {
                sr.sortingLayerName = "Player";
                wantsToBeInUnderBrush = false;
                if (onUnderbrushEvent != null)
                {
                    onUnderbrushEvent();
                }
            }
        }
    }

    void DealWithFire()
    {
        anim.SetFloat("Fire Health", fireHealth);
        if (fireHealth <= 0)
        {
            isFire = false;
            if (onFireChangeEvent != null)
            {
                onFireChangeEvent(!isFire);
            }
        }
        else
        {
            isFire = true;
            if (onFireChangeEvent != null)
            {
                onFireChangeEvent(!isFire);
            }
        }
    }

    /* Gets input from the PlayerInput class
     * @param input - input for horizontal and vertical axes.
     */
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        // Wall jumping
        //if (wallSliding)
        //{
        //    if (wallDirX == directionalInput.x)
        //    {
        //        velocity.x = -wallDirX * wallJumpClimb.x;
        //        velocity.y = wallJumpClimb.y;
        //    }
        //    else if (directionalInput.x == 0)
        //    {
        //        velocity.x = -wallDirX * wallJumpOff.x;
        //        velocity.y = wallJumpOff.y;
        //    }
        //    else // Input opposite to wall direction
        //    {
        //        velocity.x = -wallDirX * wallLeap.x;
        //        velocity.y = wallLeap.y;
        //    }
        //    isDoubleJumping = false;
        //}
        // Regular jumping
        //if (Input.GetButton("Shift"))
        //{
        //    OnShiftInput();
        //}
        //else
        //{
        if (controller.collisions.below)
        {
            isDoubleJumping = false;
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) // Not jumping against max slope
                {
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
        //}
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    /* Controls the sprint powerup, or whatever it is.
     * Basically, on the shift input, the player shoots in whatever direction the directionalInput is.
     */
    public void OnShiftInput()
    {
        if (controller.collisions.below)
        {
            isDoubleJumping = false;
        }
        if (!timeIsOut && canDoubleJump && (Input.GetButton("Jump") || Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            if ((!controller.collisions.below && !isDoubleJumping) || controller.collisions.below)
            {
                camShake.Shake(.2f, .1f);
                StartCoroutine("SprintTimer");
                Vector2 direction = new Vector2(directionalInput.x, directionalInput.y).normalized;
                velocity.x = direction.x * moveSpeed * 3;
                if (velocity.x == 0)
                {
                    velocity.y = direction.y * moveSpeed * 2f;
                }
                else
                {
                    velocity.y = direction.y * moveSpeed * 2.5f;
                }
                isDoubleJumping = true;
            }
            //if (!controller.collisions.below && !isDoubleJumping && Input.GetButtonDown("Jump"))
            //{
            //    velocity.y = maxJumpVelocity;
            //    isDoubleJumping = true;
            //    StartCoroutine("SprintTimer");
            //}
            //if(controller.collisions.below && Input.GetButtonDown("Jump"))
            //{
            //    velocity.y = maxJumpVelocity;
            //    StartCoroutine("SprintTimer");
            //}
            //if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            //{
            //    velocity.x = velocity.x + Mathf.Sign(directionalInput.x) * 10;
            //    StartCoroutine("SprintTimer");
            //}
        }
    }

    IEnumerator SprintTimer()
    {
        int max = 5;
        int initial = 0;
        while (initial < max)
        {
            initial++;
            yield return null;
        }
        timeIsOut = true;
        StopAllCoroutines();
        StartCoroutine("SprintRecharge");
    }

    IEnumerator SprintRecharge()
    {
        int max = 50;
        int initial = 0;
        while (initial < max)
        {
            initial++;
            yield return null;
        }
        timeIsOut = false;
    }

    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                wallSliding = false;
                return;
            }

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }
            // Creating a little time that the player sticks to the wall before making a leap away from the wall
            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    /* Calculates the velocity of the player depending on whether the player is on a flat area, climbing a slope,
     * or descending it. Applies a smoothing effect between the original velocity and the target velocity.
     */
    void CalculateVelocity()
    {
        float slopeAngleClimbSmoothTime = .05f + 1 / Mathf.Abs(controller.collisions.slopeAngle);
        float slopeAngleDescendSmoothTime = 0.15f + Mathf.Abs(controller.collisions.slopeAngle) * .001f;
        float targetVelocityX = directionalInput.x * moveSpeed; // For Rhino script, modify this
        if (controller.collisions.climingSlope)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, slopeAngleClimbSmoothTime);
        }
        else if (controller.collisions.descendingSlope && Mathf.Abs(velocityXOld) > 1f)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, slopeAngleDescendSmoothTime);
        }
        else
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        }

        // Sound plays when the player starts, but not necessarily when he is slowing down
        if (Mathf.Abs(velocity.x) > .5f && Mathf.Abs(velocityXOld) <= Mathf.Abs(velocity.x) && controller.collisions.below)
        {
            platformType = controller.GetPlatformType();
            if (platformType.ToString() == "grass")
            {
                gm.PlayPlatformAudio((int)platformType);
            }
        }
        /* not working
        else if (Mathf.Abs(velocity.x) > .5f && Mathf.Abs(velocity.x) <= Mathf.Abs(velocityXOld))
        {
            audioManager.PlaySound("grass01");
        }
        */

        velocityXOld = velocity.x;
        velocityYOld = velocity.y;
        velocity.y += gravity * Time.deltaTime;
    }



    public void DamageFire(int _damage)
    {
        if (fireHealth >= 0)
        {
            fireHealth -= _damage;
            anim.SetFloat("Fire Health", fireHealth);
            if (fireHealth <= 0)
            {
                isFire = false;
            }
        }
    }

    public void HealFire(float _health)
    {
        if (fireHealth < maxFireHealth)
        {
            fireHealth += _health;
            isFire = true;
            anim.SetFloat("Fire Health", fireHealth);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Done"))
        {
            SceneManager.LoadScene("Level03");
        }
    }

    public void DamagePlayer(int _damage)
    {
        health -= _damage;
        if (health < 0)
        {
            Effect();
            //if (fingPoop != null)
            //{
            //    fingPoop(); // kills the player
            //}
            GameMaster.KillPlayer(this);
        }
    }

    void Effect()
    {
        audioManager.PlaySound("PlayerDie");
        Instantiate(deathPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        //Destroy(gameObject);
    }

    void FallInWaterableObject.SetIsInWater(bool _isInWater)
    {
        isInWater = _isInWater;
    }
}
