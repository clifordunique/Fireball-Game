using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController
{
    // Variables for ascending and descending slope properly
    public float maxSlopeAngle = 80;
    float speedBySlopeAngleClimb;
    float speedBySlopeAngleDescend;

    public delegate void OnHitBranch();
    public event OnHitBranch hitBranchEvent;

    // Weird stuff
    TreeBranch treeBranch;
    Player player;
    AudioManager audioManager;

    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 playerInput;

    // Detecting platform type, mostly for playing sounds
    PlatformType platformType;
    int[] grounded;

    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;
        audioManager = AudioManager.instance;
        treeBranch = FindObjectOfType<TreeBranch>();
        player = GetComponent<Player>();
    }

    // Overloaded method for the convenience of the moving platform (PlatformController class)
    public void Move(Vector2 moveAmount, bool standingOnPlatform)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.moveAmountOld = moveAmount;
        playerInput = input;

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        if (moveAmount.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        //Always check for horizontal collisions to enable wall sliding in case player jumps while right next to a wall.
        HorizontalCollisions(ref moveAmount);
        //Do the downward raycasts if close enough to the tree
        if (treeBranch != null)
        {
            float dstToTreeBranch = Mathf.Abs(transform.position.x - treeBranch.transform.position.x);
            if (dstToTreeBranch < 50)
            {
                ConstantDownwardRaycast(moveAmount);
            }
        }

        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);
        // Added standingOnPlatform boolean so that if we are on a platform we can still jump
        if (standingOnPlatform == true)
        {
            collisions.below = true;
        }
    }

    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                // Fixes issue when platform is on top of player, decreasing the player's ability to move right and left
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    // Fixes problem when descending a slope into a narrow valley.
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        grounded = new int[verticalRayCount];

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY /*   * rayLength   */ , Color.red);

            if (hit)
            {
                if (hit.collider.tag == "FallBoundary")
                {
                    GameMaster.KillPlayer(player);
                }
                // Allows player to jump though platforms if they have the "Through" tag
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1) // Enables the player to fall through platforms with the "Through" tag
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .5f);
                        continue;
                    }
                }
                if (hit.collider.tag == "Grass")
                {
                    platformType = PlatformType.grass;
                }
                else if (hit.collider.tag == "Rock")
                {
                    platformType = PlatformType.rock;
                    if(moveAmount.y < -0.5f)
                    {
                        audioManager.PlaySound("Rock Hit");
                    }
                }

                /* Checking for a hit. If a ray hits an object, change the moveAmount to the ray length.
                 * That way the next frame the player (following its new moveAmount) should rest on the object below it.
                 * If the moveAmount doesn't change to the ray length that would be a problem b/c the player would just go through the platform.
                 */
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if (collisions.climingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    void ConstantDownwardRaycast(Vector2 moveAmount)
    {
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i * moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * -rayLength, 5, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * -rayLength /*   * rayLength   */ , Color.red);

            /* Detects if the player lands on a branch and bumps it down slightly
            */
            if (hit)
            {
                if (hit.collider.tag == "Branch" && hit.distance < .4f)
                {
                    platformType = PlatformType.treeBranch;
                    if (hitBranchEvent != null)
                    {
                        hitBranchEvent();
                    }
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 moveAmount, float _slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        speedBySlopeAngleClimb = 1 - Mathf.Abs(_slopeAngle) * .005f;
        // Added climbVelocityY and if statmemnt b/c otherwise the y moveAmount is constantly reset, disabling jumping.
        float climbVelocityY = Mathf.Sin(_slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (moveAmount.y <= climbVelocityY)
        {
            moveAmount.y = climbVelocityY * speedBySlopeAngleClimb;
            moveAmount.x = Mathf.Cos(_slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x) * speedBySlopeAngleClimb;
            // To make sure that the player can jump while climbing a slope. (Player line 37)
            collisions.below = true;
            collisions.climingSlope = true;
            collisions.slopeAngle = _slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    // Descend the slope without getting all bumpy
    // Adds varying velocity depending on slope angle
    void DescendSlope(ref Vector2 moveAmount)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }

        if (!collisions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);

            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                speedBySlopeAngleDescend = 1 + Mathf.Abs(slopeAngle) * .02f; // For increasing the velocity on a slope
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x) * speedBySlopeAngleDescend;
                            moveAmount.y -= descendVelocityY * speedBySlopeAngleDescend;

                            //UPDATE COLLISIONS
                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = hit.normal.x * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }

    public PlatformType GetPlatformType()
    {
        return platformType;
    }
}

public enum PlatformType { grass, rock, treeBranch, snow }