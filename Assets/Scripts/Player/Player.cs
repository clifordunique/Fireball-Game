/* Name: Player.cs
 * Author: Sebastian Lague
 * Modified by John Paul Depew
 * 
 * Description: This script handles a lot of the variables of the Player class.
 * It also handles wall jumping and calculating the velocity.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParentMotionBlur))]
[RequireComponent(typeof(FireEyes))]
[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, FallInWaterableObject
{
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public LayerMask underBrushLayerMask;

    public float moveSpeed = 40;
    public GameObject head;

    // WALL JUMP
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    int wallDirX;

    // UNDERBRUSH
    bool isNearUnderbrush = false;
    bool wantsToBeInUnderBrush = false;
    float timeToWallUnstick;

    float accelerationTimeAirborne = .3f;
    float accelerationTimeGrounded = .07f;

    float gravity;       // -(2 * maxJumpHeight) / timeToJumpApex^2
    float gravityOriginal;
    float maxJumpVelocity;  // gravity * timeToJumpApex
    float minJumpVelocity;
    public float maxJumpHeight;
    public Vector3 velocity;
    float velocityXSmoothing;
    public bool isInWater = false;
    public bool isDoubleJumping;
    Utilities.PlatformType platformType;
    bool grounded;

    // REFERENCES
    private GameMaster gm;
    private AudioManager audioManager;
    private CameraShake camShake;
    private PlayerStats stats;

    // REFERENCES TO COMPONENTS ON THIS OBJECT
    private FireEyes fireEyes;
    private Controller2D controller;
    private Animator anim;
    private ParentMotionBlur blur;

    public GameObject deathPrefab;
    private SpriteRenderer[] spriteRenderersInChildren;
    private Queue<GameObject> damageSpritesOnPlayer;

    //bool wallSliding;
    Vector2 directionalInput;

    // Power ups
    bool timeIsOut = false;

    //CameraShake variables
    public float camShakeAmt = 0.1f;
    public float camShakeLength = 0.1f;

    float velocityXOld;

    // DELEGATES
    public delegate void OnFireChange(bool currentFire);
    public event OnFireChange onFireChangeEvent;

    public delegate void OnUnderbrush();
    public event OnUnderbrush onUnderbrushEvent;

    void Start()
    {
        // REFERENCES
        gm = GameMaster.gm;
        if (gm == null)
        {
            Debug.Log("No GameMaster found in scene.");
        }
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("No AudioManager found in scene.");
        }
        camShake = gm.GetComponent<CameraShake>();
        if (camShake == null)
        {
            Debug.LogError("No CameraShake found on the GameMaster.");
        }
        stats = PlayerStats.instance;
        if (stats == null)
        {
            Debug.LogError("No PlayerStats found in scene.");
        }

        // REFERENCES TO COMPONENTS ON THIS OBJECT
        fireEyes = GetComponent<FireEyes>();
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
        blur = GetComponent<ParentMotionBlur>();

        spriteRenderersInChildren = GetComponentsInChildren<SpriteRenderer>();
        damageSpritesOnPlayer = new Queue<GameObject>();

        stats.CurFireHealth = stats.MaxFireHealth;
        stats.CurHealth = stats.MaxHealth;
        stats.onHealEvent += OnPlayerHeal;
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        gravityOriginal = gravity;
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update()
    {
        // First check and see if the state is paused. If it is, return.
        if (gm.CurState == Utilities.State.PAUSED) return;

        CalculateVelocity();
        //HandleWallSliding();
        DetectUnderBrush();

        if (controller.collisions.below)
        {
            isDoubleJumping = false;
        }
        controller.Move(velocity * Time.deltaTime, directionalInput, isDoubleJumping);

        // Calculating gravity
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

    private void OnPlayerHeal()
    {
        if (damageSpritesOnPlayer.Count > 0)
        {
            Utilities.instance.FadeObjectOut(damageSpritesOnPlayer.Dequeue(), 0.02f, true, false);
        }
    }

    void DetectUnderBrush()
    {
        var srs = GetComponentsInChildren<SpriteRenderer>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1f, underBrushLayerMask);
        if (hit)
        {
            if (hit.collider.CompareTag("Underbrush"))
            {
                isNearUnderbrush = true;
                // This if is so that if the player goes out of the underbrush but quickly goes back in he won't suddenly be in the Player layer
                if (wantsToBeInUnderBrush)
                {
                    for (int i = 0; i < srs.Length; i++)
                    {
                        srs[i].sortingLayerName = "Behind Underbrush";
                    }
                }
            }
        }
        else
        {
            isNearUnderbrush = false;
            for (int i = 0; i < srs.Length; i++)
            {
                srs[i].sortingLayerName = "Player";
            }
        }
    }

    /* Toggles if the player is behind the underbrush
    */
    public void ToggleIsInUnderbrush()
    {
        if (isNearUnderbrush)
        {
            audioManager.PlaySound("underbrush");
            if (spriteRenderersInChildren[0].sortingLayerName == "Player")
            {
                for (int i = 0; i < spriteRenderersInChildren.Length; i++)
                {
                    spriteRenderersInChildren[i].sortingLayerName = "Behind Underbrush";
                }
                wantsToBeInUnderBrush = true;
                if (onUnderbrushEvent != null)
                {
                    onUnderbrushEvent();
                }
            }
            else
            {
                for (int i = 0; i < spriteRenderersInChildren.Length; i++)
                {
                    spriteRenderersInChildren[i].sortingLayerName = "Player";
                }
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
        // value between 0 and 1 to correctly set variables in the blend tree
        // In other words, the player walks faster or slower depending on speed.
        // However, it doesn't matter if the player is going up or down hill... dang it
        float speedValue = velocity.sqrMagnitude / (moveSpeed * moveSpeed + 5);
        anim.SetFloat("Speed", Mathf.Abs(speedValue));
        anim.SetBool("Grounded", controller.collisions.below);
        fireEyes.SetFireBase(stats.CurFireHealth, stats.MaxFireHealth);

        if (stats.CurFireHealth <= 0)
        {
            if (onFireChangeEvent != null)
            {
                onFireChangeEvent(true);
            }
        }
        else
        {
            if (onFireChangeEvent != null)
            {
                onFireChangeEvent(false);
            }
        }
    }

    /* Gets input from the PlayerInput class
     * @param input - input for horizontal and vertical axes.
     */
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
        if (directionalInput.x != 0)
        {
            if (directionalInput.x > 0)
            {
                transform.localScale = new Vector2(1, transform.localScale.y);
            }
            else
            {
                transform.localScale = new Vector2(-1, transform.localScale.y);
            }
        }
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
        //Debug.Log(!timeIsOut + " "  + stats.Zoom + " " +  Input.GetButton("Jump") + " "+ Input.GetButton("Horizontal") + " " + Input.GetButton("Vertical"));
        if (!timeIsOut && stats.Zoom && (Input.GetButton("Jump") || Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            if ((!controller.collisions.below && !isDoubleJumping) || controller.collisions.below)
            {
                StartCoroutine("SprintTimer");
                audioManager.PlaySound("Zoom");
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
        }
    }

    IEnumerator SprintTimer()
    {
        audioManager.PlaySound("Zoom");
        int max = 5;
        int initial = 0;
        while (initial < max)
        {
            blur.Blur();
            initial++;
            yield return new WaitForSeconds(0.01f);
        }
        timeIsOut = true;

        StopAllCoroutines();
        StartCoroutine("SprintRecharge");
    }

    IEnumerator SprintRecharge()
    {
        int max = 20;
        int initial = 0;
        while (initial < max)
        {
            initial++;
            yield return null;
        }
        timeIsOut = false;
    }

    // I'm not sure if I'll do wall sliding or not...
    //
    //void HandleWallSliding()
    //{
    //    wallDirX = (controller.collisions.left) ? -1 : 1;
    //    wallSliding = false;
    //    if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
    //    {
    //        wallSliding = true;
    //        if (Input.GetKey(KeyCode.DownArrow))
    //        {
    //            wallSliding = false;
    //            return;
    //        }

    //        if (velocity.y < -wallSlideSpeedMax)
    //        {
    //            velocity.y = -wallSlideSpeedMax;
    //        }
    //        // Creating a little time that the player sticks to the wall before making a leap away from the wall
    //        if (timeToWallUnstick > 0)
    //        {
    //            velocityXSmoothing = 0;
    //            velocity.x = 0;

    //            if (directionalInput.x != wallDirX && directionalInput.x != 0)
    //            {
    //                timeToWallUnstick -= Time.deltaTime;
    //            }
    //            else
    //            {
    //                timeToWallUnstick = wallStickTime;
    //            }
    //        }
    //        else
    //        {
    //            timeToWallUnstick = wallStickTime;
    //        }
    //    }
    //}

    /* Calculates the velocity of the player depending on whether the player is on a flat area, climbing a slope,
     * or descending it. Applies a smoothing effect between the original velocity and the target velocity.
     * Values / Objects used: Controller2D, moveSpeed, directionalInput, velocityXOld
     */
    void CalculateVelocity()
    {
        // Affecting player speed when on slopes
        float safetyValue = 0.05f;
        float maxDiffDivisor = 1 / safetyValue;
        float slopeAngleBtw0and1 = Mathf.Abs(controller.collisions.slopeAngle) / controller.maxSlopeAngle + safetyValue;
        float diffValue = (1 / Mathf.Abs(1 - slopeAngleBtw0and1)) / maxDiffDivisor; // Weirdo equation for calculating a multiplier for climbing/descending slopes
        float climbMultiplier = 1 - diffValue;
        float descendMultiplier = 1 + diffValue;

        float targetVelocityX = directionalInput.x * moveSpeed;

        // This doesn't take account for actual speed
        if (controller.collisions.climingSlope)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX * climbMultiplier, ref velocityXSmoothing, accelerationTimeGrounded * climbMultiplier);
        }
        else if (controller.collisions.descendingSlope && Mathf.Abs(velocityXOld) > 1f)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX * descendMultiplier, ref velocityXSmoothing, accelerationTimeGrounded * descendMultiplier * 1.5f);
        }
        else
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        }

        // Sound plays when the player starts, but not necessarily when he is slowing down
        if (Mathf.Abs(velocity.x) > 0.5f && Mathf.Abs(velocityXOld) <= Mathf.Abs(velocity.x) && controller.collisions.below)
        {
            platformType = controller.GetPlatformType();
            gm.PlayPlatformAudio(platformType, velocity.magnitude, moveSpeed);
        }

        // Landing after jumping 
        if (grounded == false && controller.collisions.below)
        {
            grounded = true;
            platformType = controller.GetPlatformType();
            gm.PlayPlatformAudio(platformType, velocity.magnitude, moveSpeed);
        }
        else if (!controller.collisions.below)
        {
            grounded = false;
        }

        velocityXOld = velocity.x;
        velocity.y += gravity * Time.deltaTime;
    }

    public void DamageFire(int _damage)
    {
        if (stats.CurFireHealth >= 0)
        {
            stats.CurFireHealth -= _damage;
        }
    }

    /// <summary>
    /// Damages the player. If health equals 0, it kills the player.
    /// </summary>
    /// <param name="data"></param>
    public void DamagePlayer(DamagePlayerData data)
    {
        stats.CurHealth -= data.damageToPlayerHealth;
        stats.CurFireHealth -= data.damageToPlayerFireHealth;
        if (data.damagePlayerEffect != null)
        {
            GameObject tempDamagePlayerEffect = Instantiate(data.damagePlayerEffect, data.hitPos, data.transformInfo.rotation, head.transform);
            damageSpritesOnPlayer.Enqueue(tempDamagePlayerEffect);
        }

        if (stats.CurHealth <= 0)
        {
            Effect();
            GameMaster.KillPlayer(this);
        }
    }

    public void HealFire(int _health)
    {
        if (stats.CurFireHealth < stats.MaxFireHealth)
        {
            stats.CurFireHealth += _health;
        }
    }


    void Effect()
    {
        audioManager.PlaySound("PlayerDie");
        Instantiate(deathPrefab, transform.position, Quaternion.Euler(0, 0, 0));
    }

    void FallInWaterableObject.SetIsInWater(bool _isInWater)
    {
        isInWater = _isInWater;
    }

    public bool isClimbingSlope()
    {
        return controller.collisions.descendingSlope;
    }
}
