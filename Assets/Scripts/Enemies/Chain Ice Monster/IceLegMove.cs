using System.Collections;
using UnityEngine;

public class IceLegMove : Enemy
{
    public IceLeg[] iceLegs;
    public Transform head;
    public float moveUpAmount = 5f;
    public float moveUpSpeed = 0.2f;
    public float moveSideAmount = 30f;
    public float moveSideSpeed = 0.5f;
    public float moveDownSpeed = 1.4f;
    public float legCallbackDst = 20f;
    public float crippleWaitTime = 0.4f;
    public float tooFarApartDst = 20f;

    public Player player;
    public GameObject playerScratch;
    public float seePlayerDst = 10;

    public float rayLength = 0.5f;
    public LayerMask groundImpactMask;
    public LayerMask groundDetectorMask;
    public LayerMask playerMask;

    CameraShake camShake;
    AudioManager audioManager;

    // sounds
    string[] smallShakeSounds;
    string[] moveFastSounds;
    private string[] pullSounds;

    private static float deadLegs = 0;
    public delegate void OnAllLegsDead();
    public static event OnAllLegsDead onAllLegsDead;

    public override void Start()
    {
        base.Start();
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        audioManager = AudioManager.instance;

        onEnemyDestroy += EnemyIsDestroyed;

        InitializeChainSounds();

        StartCoroutine(Walk());
        damagePlayerData.damagePlayerEffect = playerScratch;
    }

    private void InitializeChainSounds()
    {
        smallShakeSounds = new string[7];
        moveFastSounds = new string[3];
        pullSounds = new string[8];
        for (int i = 0; i < smallShakeSounds.Length; i++)
        {
            smallShakeSounds[i] = "chainSmall0" + (i + 1);
        }
        for (int i = 0; i < moveFastSounds.Length; i++)
        {
            moveFastSounds[i] = "chainMove0" + (i + 1);
        }
        for (int i = 0; i < pullSounds.Length; i++)
        {
            pullSounds[i] = "chainPull0" + (i + 1);
        }
    }

    /// <summary>
    /// Gets a random index between 0 and max
    /// </summary>
    /// <param name="max">The maximum value for the random index</param>
    /// <returns></returns>
    private int GetRandomIndex(int max)
    {
        return Random.Range(0, max);
    }

    bool CanSeePlayer()
    {
        if (player != null)
        {
            Vector2 dirToPlayer = (player.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, seePlayerDst, playerMask);
            //Debug.DrawRay(transform.position, dirToPlayer * seePlayerDst, Color.red);
            if (hit)
            {
                return hit.transform.tag == "Player";
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    IEnumerator Walk()
    {
        while (true)
        {
            for (int i = 0; i < iceLegs.Length; i++)
            {
                if (iceLegs[i].health > 0)
                {
                    Vector2 dstFromHead = iceLegs[i].transform.position - transform.position;
                    float dstToMove = moveSideAmount;
                    bool tooFarFromHead = false;

                    if (dstFromHead.magnitude > tooFarApartDst)
                    {
                        dstToMove = dstFromHead.x;
                        tooFarFromHead = true;
                    }
                    bool seePlayer;

                    if (player != null)
                    {
                        seePlayer = CanSeePlayer();
                    }
                    else
                    {
                        seePlayer = false;
                    }
                    yield return MoveUp(i);
                    if (seePlayer && !tooFarFromHead)
                    {
                        StartCoroutine(MoveToPlayer(i));
                    }
                    else
                    {
                        yield return MoveToPos(i, tooFarFromHead);
                        yield return MoveDown(i);
                    }
                }
                else
                {
                    yield return new WaitForSeconds(crippleWaitTime);
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator MoveUp(int currentLegIndex)
    {
        if (iceLegs[currentLegIndex] != null)
        {
            int index = GetRandomIndex(pullSounds.Length);
            float totalMoveDisplacement = 0;
            float moveUpSpeedSlower = moveUpSpeed * 0.5f;
            float dirToPlayerX = 1;
            bool shouldPointAtPlayer = true;

            IceLeg currentLeg = iceLegs[currentLegIndex];
            RaycastHit2D hit = currentLeg.GetGroundDetectorHit(groundDetectorMask);

            audioManager.PlaySound(pullSounds[index]);


            if (player != null)
            {
                dirToPlayerX = Mathf.Sign(player.transform.position.x - currentLeg.transform.position.x);
            }
            else
            {
                dirToPlayerX = -1;
            }


            while (currentLeg != null && totalMoveDisplacement < moveUpAmount)
            {
                if (CanSeePlayer())
                {
                    currentLeg.transform.Translate(Vector2.right * dirToPlayerX * .25f, Space.World);
                    currentLeg.transform.Translate(hit.normal * moveUpSpeedSlower, Space.World);

                    RaycastHit2D sideHit = currentLeg.GetSideHit(groundDetectorMask, 1, false);
                    if ((!sideHit || (sideHit && sideHit.distance != 0)) && shouldPointAtPlayer)
                    {
                        PointAtPlayer(currentLegIndex);
                    }
                    else
                    {
                        shouldPointAtPlayer = false;
                        //Quaternion angle = Quaternion.FromToRotation(Vector2.up, sideHit.normal);
                        //currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, angle, 0.2f);
                        totalMoveDisplacement += moveUpSpeed;
                        currentLeg.transform.Translate(hit.normal * moveUpSpeed, Space.World);
                    }
                    totalMoveDisplacement += moveUpSpeedSlower;
                }
                else
                {
                    totalMoveDisplacement += moveUpSpeed;
                    currentLeg.transform.Translate(hit.normal * moveUpSpeed, Space.World);
                }

                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    IEnumerator MoveToPos(int currentLegIndex, bool tooFarFromHead)
    {
        if (iceLegs[currentLegIndex] != null)
        {
            int index = GetRandomIndex(smallShakeSounds.Length);
            audioManager.PlaySound(smallShakeSounds[index]);

            IceLeg currentLeg = iceLegs[currentLegIndex];
            float dirToMove;
            if (player != null)
            {
                if (!tooFarFromHead)
                {
                    dirToMove = Mathf.Sign(player.transform.position.x - currentLeg.transform.position.x);
                }
                else // If the legs are too far from the head, they will move back towards the head.
                {
                    dirToMove = Mathf.Sign(transform.position.x - currentLeg.transform.position.x);
                }
            }
            else
            {
                dirToMove = -1;
            }
            //float dirToPlayerY = Mathf.Sign(player.transform.position.y - currentLeg.transform.position.y);

            //float moveSpeed = .3f;
            RaycastHit2D hit = currentLeg.GetGroundDetectorHit(groundDetectorMask);

            float counter = 0;

            // move a little in the player's direction
            if (hit)
            {
                while (currentLeg != null && counter < moveSideAmount && !currentLeg.GetHit(groundImpactMask))
                {
                    RaycastHit2D sideHit = currentLeg.GetSideHit(groundDetectorMask, dirToMove, true);
                    if (!sideHit)
                    {
                        hit = currentLeg.GetGroundDetectorHit(groundDetectorMask);
                    }
                    else
                    {
                        hit = currentLeg.GetSideHit(groundDetectorMask, dirToMove, true);
                    }
                    Quaternion angle = Quaternion.FromToRotation(Vector2.up, hit.normal);
                    counter += moveSideSpeed;

                    // Movement
                    currentLeg.transform.Translate(Vector2.right * dirToMove * moveSideSpeed);
                    currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, angle, 0.2f);

                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                Quaternion downwardAngle = Quaternion.AngleAxis(0, Vector3.forward);

                while (currentLeg != null && (currentLeg.transform.rotation != downwardAngle || counter < moveSideAmount))
                {
                    counter += moveSideSpeed;
                    currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, downwardAngle, Time.deltaTime * 20);
                    currentLeg.transform.Translate(Vector2.right * dirToMove * moveSideSpeed);

                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }

    /// <summary>
    /// Points the leg at the player
    /// </summary>
    /// <param name="currentLegIndex">The index of the leg to rotate</param>
    /// <returns></returns>
    void PointAtPlayer(int currentLegIndex)
    {
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            Transform currentLeg = iceLegs[currentLegIndex].transform;
            float speed = 20f;
            Vector3 vectorToTarget = currentLeg.position - player.transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion angleToPlayer = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, angleToPlayer, Time.deltaTime * speed);
        }
    }

    /// <summary>
    /// Moves the ice leg directly towards the player
    /// </summary>
    /// <param name="currentLegIndex">The index of the leg</param>
    /// <returns></returns>
    IEnumerator MoveToPlayer(int currentLegIndex)
    {
        bool canDamagePlayer = true;
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            int index = GetRandomIndex(moveFastSounds.Length);

            IceLeg currentLeg = iceLegs[currentLegIndex];
            Vector2 dirToPlayer = (player.transform.position - currentLeg.transform.position).normalized;
            Vector2 dirToGround = currentLeg.GetDownwardsDirection();
            RaycastHit2D hit = currentLeg.GetHit(groundImpactMask);
            float moveSlowerSpeed = moveDownSpeed * 0.3f;
            bool shouldPointAtPlayer = true;
            float timer = 30;
            float count = 0;

            float volume = currentLeg.GetComponent<AudioSource>().volume;

            audioManager.PlaySound(moveFastSounds[index]);
            StopCoroutine("MoveUp");

            while (currentLeg != null)
            {
                if (hit)
                {
                    if (canDamagePlayer && hit.collider.CompareTag("Player"))
                    {
                        damagePlayerData.transformInfo = currentLeg.transform;
                        damagePlayerData.hitPos = hit.point;


                        //Instantiate(playerScratch, hit.point, currentLeg.transform.rotation, player.head.transform);
                        player.DamagePlayer(damagePlayerData);
                        camShake.Shake(0.08f, 0.08f);
                        canDamagePlayer = false;
                        audioManager.PlaySound("Ice Hit");
                    }
                    if (hit.collider.CompareTag("Snow"))
                    {
                        camShake.Shake(0.05f, 0.05f);

                        currentLeg.GetComponent<AudioSource>().volume = volume;
                        currentLeg.PlayAudio();
                        break;
                    }
                }
                if (count > timer)
                {
                    break;
                }

                //Debug.Log(Mathf.Round(dirToPlayer.x * 10 / 10) != Mathf.Round(dirToGround.x * 10 / 10) && Mathf.Round(dirToPlayer.y * 10 / 10) != Mathf.Round(dirToGround.y * 10 / 10));
                // Movement
                // It is pointing upwards or something and so it shouldn't just be rushed down
                if (Mathf.Round(dirToPlayer.x * 10 / 10) != Mathf.Round(dirToGround.x * 10 / 10) && Mathf.Round(dirToPlayer.y * 10 / 10) != Mathf.Round(dirToGround.y * 10 / 10))
                {
                    currentLeg.transform.Translate(Vector2.down * moveSlowerSpeed);

                    RaycastHit2D sideHit = currentLeg.GetSideHit(groundDetectorMask, 1, false);
                    if ((!sideHit || (sideHit && sideHit.distance != 0)) && shouldPointAtPlayer)
                    {
                        PointAtPlayer(currentLegIndex);
                    }
                    else
                    {
                        StartCoroutine("MoveUp", currentLegIndex);
                        break;
                        //shouldPointAtPlayer = false;
                        //Quaternion angle = Quaternion.FromToRotation(Vector2.up, sideHit.normal);
                        //currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, angle, 0.5f);
                    }
                }
                else
                {
                    currentLeg.transform.Translate(Vector2.down * moveDownSpeed);
                }

                // get values
                hit = currentLeg.GetHit(groundImpactMask);
                count++;

                yield return new WaitForSeconds(.01f);
            }
            if (currentLeg != null)
            {
                currentLeg.GetComponent<AudioSource>().volume = volume;
            }
        }
        canDamagePlayer = false;
    }

    /// <summary>
    /// Moves the leg at currentLegIndex down until it detects a collision with an object
    /// </summary>
    /// <param name="currentLegIndex">The index of the leg to move down</param>
    /// <returns></returns>
    IEnumerator MoveDown(int currentLegIndex)
    {
        bool damagePlayer = true;

        if (iceLegs[currentLegIndex] != null)
        {
            int index = GetRandomIndex(smallShakeSounds.Length);
            audioManager.PlaySound(smallShakeSounds[index]);

            IceLeg currentLeg = iceLegs[currentLegIndex];
            float moveSpeed = 1.5f;
            RaycastHit2D hit = currentLeg.GetHit(groundImpactMask);

            float timer = 50;
            float count = 0;

            while (currentLeg != null)
            {
                if (hit)
                {
                    if (hit.collider.CompareTag("Snow"))
                    {
                        camShake.Shake(0.05f, 0.05f);
                        currentLeg.PlayAudio();
                        break;
                    }
                    else if (damagePlayer && hit.collider.CompareTag("Player"))
                    {
                        damagePlayerData.transformInfo = currentLeg.transform;
                        damagePlayerData.hitPos = hit.point;

                        player.DamagePlayer(damagePlayerData);
                        camShake.Shake(0.08f, 0.08f);
                        damagePlayer = false;
                        currentLeg.PlayAudio(); // this will have to be changed to the player hit sound
                    }
                }
                if (count > timer)
                {
                    break;
                }

                // Movement
                currentLeg.transform.Translate(Vector2.down * moveSpeed);

                // getting values
                hit = currentLeg.GetHit(groundImpactMask);
                count++;

                yield return new WaitForSeconds(.01f);

            }
            damagePlayer = false;
        }
    }

    /// <summary>
    /// This handles the event when a leg is destroyed.
    /// It increments deadLegs so that this script can keep track of
    /// how many legs have been destroyed.
    /// </summary>
    /// <param name="gameObject">The enemy that was destroyed</param>
    private void EnemyIsDestroyed(GameObject gameObject)
    {
        if (gameObject.GetComponent<IceLeg>() != null)
        {
            deadLegs++;
            if (deadLegs >= iceLegs.Length)
            {
                if (onAllLegsDead != null)
                {
                    onAllLegsDead();
                }
            }
        }
    }
}