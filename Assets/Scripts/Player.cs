/* Name: Player.cs
 * Author: Sebastian Lague
 * Modified by John Paul Depew
 * 
 * Description: This script handles a lot of the variables of the Player class.
 * It also handles wall jumping and calculating the velocity.
 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .3f;
    float accelerationTimeGrounded = .2f;
    float accelerationTimeDescendingSlope = .5f;
    float accelerationTimeClimbingSlope = .1f;
    public float moveSpeed = 40;

    public float maxHealth = 10;
    public float health;
    public float maxFireStrength = 10;
    public float fireStrength;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    float gravity;       // -(2 * maxJumpHeight) / timeToJumpApex^2
    float maxJumpVelocity;  // gravity * timeToJumpApex
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    CollisionInfo colInfo;
    Controller2D controller;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    float velocityXOld;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        health = maxHealth;
        fireStrength = maxFireStrength;
        //print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
    }

    void Update()
    {
        CalculateVelocity();
        HandleWallSliding();

        controller.Move(velocity * Time.deltaTime, directionalInput);

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
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else // Input opposite to wall direction
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        // Regular jumping
        if (controller.collisions.below)
        {
            if(controller.collisions.slidingDownMaxSlope)
            {
                if(directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) // Not jumping against max slope
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
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
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
        float slopeAngleClimbSmoothTime = .05f + 1/Mathf.Abs(controller.collisions.slopeAngle);
        float slopeAngleDescendSmoothTime = .15f + Mathf.Abs(controller.collisions.slopeAngle) * .01f;
        float targetVelocityX = directionalInput.x * moveSpeed;
        if(controller.collisions.climingSlope)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, slopeAngleClimbSmoothTime);
            Debug.Log(controller.collisions.slopeAngle);
            //Debug.Log("climb: " + slopeAngleClimbSmoothTime);
        }
        else if(controller.collisions.descendingSlope && Mathf.Abs(velocityXOld) > 1f)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, slopeAngleDescendSmoothTime);
            //Debug.Log(controller.collisions.slopeAngle);
            //Debug.Log("descend: " + slopeAngleDescendSmoothTime);
        }
        else
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        }

        velocityXOld = velocity.x;
        velocity.y += gravity * Time.deltaTime;
    }

    public void DamageFire(int _damage)
    {
        fireStrength -= _damage;
    }

    public void DamagePlayer(int _damage)
    {
        health -= _damage;
    }
}
