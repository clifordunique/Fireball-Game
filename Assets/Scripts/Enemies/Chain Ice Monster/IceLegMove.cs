﻿using System.Collections;
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

    public Vector3 rayStartPos;
    public float rayLength = 0.5f;
    public LayerMask layerMask;

    private bool damagePlayer = false;

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
                if (iceLegs[i].health > 0 && player != null)
                {
                    bool seePlayer = Mathf.Abs(head.transform.position.x - player.transform.position.x) < seePlayerDst;
                    yield return MoveUp(i);
                    yield return MoveToPos(i);
                    if (seePlayer)
                    {
                        yield return PointAtPlayer(i);
                        yield return MoveToPlayer(i);
                    }
                    else
                    {
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
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            IceLeg currentLeg = iceLegs[currentLegIndex];
            float targetPosY = currentLeg.transform.position.y + moveUpAmount;
            float moveSpeed = 0.3f;

            StartCoroutine("RotateDownwards", currentLegIndex);

            while (currentLeg != null && currentLeg.transform.position.y < targetPosY)
            {
                currentLeg.transform.position = new Vector3(currentLeg.transform.position.x, currentLeg.transform.position.y + moveSpeed, currentLeg.transform.position.z);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    IEnumerator MoveToPos(int currentLegIndex)
    {
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            IceLeg currentLeg = iceLegs[currentLegIndex];
            float dirToPlayer = Mathf.Sign(player.transform.position.x - currentLeg.transform.position.x);
            float targetPosX = currentLeg.transform.position.x + stepDst * dirToPlayer;
            float moveSpeed = 0.3f;

            while (currentLeg != null && currentLeg.transform.position.x * dirToPlayer < targetPosX * dirToPlayer)
            {
                currentLeg.transform.position = new Vector3(currentLeg.transform.position.x + moveSpeed * dirToPlayer, currentLeg.transform.position.y, currentLeg.transform.position.z);
                yield return new WaitForSeconds(0.01f);
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

            StopCoroutine("RotateDownwards");

            while (currentLeg != null && (currentLeg.transform.rotation.eulerAngles.z > angleToPlayer.eulerAngles.z + 0.1f || currentLeg.transform.rotation.eulerAngles.z < angleToPlayer.eulerAngles.z - 0.1f))
            {
                if (iceLegs[currentLegIndex] != null)
                {
                    currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, angleToPlayer, Time.deltaTime * speed);
                    yield return new WaitForSeconds(0.01f);
                }
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
        damagePlayer = true;
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            IceLeg currentLeg = iceLegs[currentLegIndex];
            Vector2 dirToPlayer = player.transform.position - currentLeg.transform.position;
            dirToPlayer.Normalize();
            float dstFromHead = Mathf.Abs((head.transform.position - currentLeg.transform.position).magnitude);
            float moveSpeed = 1.4f;
            bool hitGround = false;

            while (currentLeg != null && !hitGround)
            {
                if (currentLeg.hit)
                {
                    if (currentLeg.hit.collider.CompareTag("Player") && damagePlayer)
                    {
                        player.DamagePlayer(damageToPlayerHealth);
                        camShake.Shake(0.08f, 0.08f);
                        damagePlayer = false;
                    }
                    else
                    {
                        camShake.Shake(0.08f, 0.08f);
                        hitGround = true;
                        break;
                    }
                }
                currentLeg.transform.Translate(Vector2.down * moveSpeed);
                if (dstFromHead > legCallbackDst)
                {
                    break;
                }
                dstFromHead = Mathf.Abs((head.transform.position - currentLeg.transform.position).magnitude);

                yield return new WaitForSeconds(.01f);
            }
            damagePlayer = false;
            currentLeg.PlayAudio();
        }

    }

    /// <summary>
    /// Moves the leg at currentLegIndex down until it detects a collision with an object
    /// </summary>
    /// <param name="currentLegIndex">The index of the leg to move down</param>
    /// <returns></returns>
    IEnumerator MoveDown(int currentLegIndex)
    {
        damagePlayer = true;
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            IceLeg currentLeg = iceLegs[currentLegIndex];
            float moveSpeed = 1.4f;
            bool hitGround = false;
            float dstFromHead = Mathf.Abs((head.transform.position - currentLeg.transform.position).magnitude);

            while (currentLeg != null && !hitGround)
            {
                if (currentLeg.hit)
                {
                    if (currentLeg.hit.collider.CompareTag("Player") && damagePlayer)
                    {
                        player.DamagePlayer(damageToPlayerHealth);
                        camShake.Shake(0.08f, 0.08f);
                        damagePlayer = false;
                    }
                    else
                    {
                        camShake.Shake(0.08f, 0.08f);
                        hitGround = true;
                        break;
                    }
                }
                currentLeg.transform.position = new Vector3(currentLeg.transform.position.x, currentLeg.transform.position.y - moveSpeed, currentLeg.transform.position.z);
                // This makes sure that if he walks off a cliff he doesn't fall forever
                dstFromHead = Mathf.Abs((head.transform.position - currentLeg.transform.position).magnitude);
                if (dstFromHead > legCallbackDst)
                {
                    break;
                }
                yield return new WaitForSeconds(.01f);
            }
            damagePlayer = false;
            currentLeg.PlayAudio();
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
