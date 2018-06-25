using System.Collections;
using UnityEngine;

public class IceLegMove : Enemy
{
    public IceLeg[] iceLegs;
    public Transform head;
    public float moveUpAmount = 5f;
    public float stepDst = 5f;
    public float legCallbackDst = 20f;
    public float crippleWaitTime = 0.4f;

    public Player player;
    public float seePlayerDst = 10;

    public float rayLength = 0.5f;
    public LayerMask groundImpactMask;
    public LayerMask groundDetectorMask;

    CameraShake camShake;
    AudioManager audioManager;

    public override void Start()
    {
        base.Start();
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        StartCoroutine(Walk());
        audioManager = AudioManager.instance;
    }

    IEnumerator Walk()
    {
        while (true)
        {
            for (int i = 0; i < iceLegs.Length; i++)
            {
                if (iceLegs[i].health > 0)
                {
                    bool seePlayer;
                    if (player != null)
                    {
                        seePlayer = Mathf.Abs(head.transform.position.x - player.transform.position.x) < seePlayerDst;
                    }
                    else
                    {
                        seePlayer = false;
                    }
                    yield return MoveUp(i);
                    if (seePlayer)
                    {
                        yield return PointAtPlayer(i);
                        yield return MoveToPlayer(i);
                    }
                    else
                    {
                        yield return MoveToPos(i);
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
            IceLeg currentLeg = iceLegs[currentLegIndex];
            float moveSpeed = 0.15f;
            float totalMoveDisplacement = 0;

            RaycastHit2D hit = currentLeg.GetGroundDetectorHit(groundDetectorMask);

            while (currentLeg != null && totalMoveDisplacement < moveUpAmount)
            {
                totalMoveDisplacement += moveSpeed;

                currentLeg.transform.Translate(hit.normal * moveSpeed, Space.World);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    IEnumerator MoveToPos(int currentLegIndex)
    {
        if (iceLegs[currentLegIndex] != null)
        {
            IceLeg currentLeg = iceLegs[currentLegIndex];
            float dirToPlayerX;
            if (player != null)
            {
                dirToPlayerX = Mathf.Sign(player.transform.position.x - currentLeg.transform.position.x);
            }
            else
            {
                dirToPlayerX = -1;
            }
            //float dirToPlayerY = Mathf.Sign(player.transform.position.y - currentLeg.transform.position.y);

            float moveSpeed = .3f;
            RaycastHit2D hit = currentLeg.GetGroundDetectorHit(groundDetectorMask);

            float moveAmount = 30f;
            float counter = 0;

            // move a little in the player's direction
            if (hit)
            {
                while (currentLeg != null && counter < moveAmount && !currentLeg.GetHit(groundImpactMask))
                {

                    // Get values
                    hit = currentLeg.GetGroundDetectorHit(groundDetectorMask);
                    Quaternion angle = Quaternion.FromToRotation(Vector2.up, hit.normal);
                    counter++;

                    // Movement
                    currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, angle, 0.2f);
                    currentLeg.transform.Translate(Vector2.right * dirToPlayerX * moveSpeed);

                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                Quaternion downwardAngle = Quaternion.AngleAxis(0, Vector3.forward);

                while (currentLeg != null && currentLeg.transform.rotation != downwardAngle)
                {
                    currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, downwardAngle, Time.deltaTime * 20);
                    currentLeg.transform.Translate(Vector2.right * dirToPlayerX * moveSpeed);

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
    IEnumerator PointAtPlayer(int currentLegIndex)
    {
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            Transform currentLeg = iceLegs[currentLegIndex].transform;
            float speed = 20f;
            Vector3 vectorToTarget = currentLeg.position - player.transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion angleToPlayer = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            while (currentLeg != null && (currentLeg.transform.rotation.eulerAngles.z > angleToPlayer.eulerAngles.z + 0.1f || currentLeg.transform.rotation.eulerAngles.z < angleToPlayer.eulerAngles.z - 0.1f))
            {
                currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, angleToPlayer, Time.deltaTime * speed);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    /// <summary>
    /// Moves the ice leg directly towards the player
    /// </summary>
    /// <param name="currentLegIndex">The index of the leg</param>
    /// <returns></returns>
    IEnumerator MoveToPlayer(int currentLegIndex)
    {
        bool damagePlayer = true;
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            IceLeg currentLeg = iceLegs[currentLegIndex];
            Vector2 dirToPlayer = player.transform.position - currentLeg.transform.position;
            dirToPlayer.Normalize();
            float moveSpeed = 1.4f;
            RaycastHit2D hit = currentLeg.GetHit(groundImpactMask);

            float timer = 30;
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
                    if (damagePlayer && hit.collider.CompareTag("Player"))
                    {
                        player.DamagePlayer(damageToPlayerHealth);
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

                // get values
                hit = currentLeg.GetHit(groundImpactMask);
                count++;

                yield return new WaitForSeconds(.01f);
            }
        }
        damagePlayer = false;

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
                        player.DamagePlayer(damageToPlayerHealth);
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

    IEnumerator RotateDownwards(int currentLegIndex)
    {
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            Transform currentLeg = iceLegs[currentLegIndex].transform;
            Quaternion downwardAngle = Quaternion.AngleAxis(0, Vector3.forward);

            while (currentLeg != null && currentLeg.transform.rotation != downwardAngle)
            {
                currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, downwardAngle, Time.deltaTime * 5);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
