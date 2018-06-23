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
    public LayerMask groundLayerMask;

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
                    if (seePlayer)
                    {
                        yield return PointAtPlayer(i);
                        yield return MoveToPlayer(i);
                    }
                    else
                    {
                        yield return MoveToPos(i);
                        //yield return RotateToGround(i);
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
        Debug.Log("MoveUp");
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            IceLeg currentLeg = iceLegs[currentLegIndex];
            float targetPosY = currentLeg.transform.position.y + moveUpAmount;
            float moveSpeed = 0.15f;
            float totalMoveDisplacement = 0;

            Debug.DrawRay(currentLeg.transform.position, Vector2.down * 30, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(currentLeg.transform.position, Vector2.down, 30, groundLayerMask);

            while (currentLeg != null && totalMoveDisplacement < moveUpAmount)
            {
                //Debug.Log(totalMoveDisplacement + " " + moveUpAmount);
                totalMoveDisplacement += moveSpeed;
                
                currentLeg.transform.Translate(hit.normal * moveSpeed,Space.World);
                //currentLeg.transform.position = new Vector3(currentLeg.transform.position.x, currentLeg.transform.position.y + moveSpeed, currentLeg.transform.position.z);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    IEnumerator MoveToPos(int currentLegIndex)
    {
        Debug.Log("MoveToPos");
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            IceLeg currentLeg = iceLegs[currentLegIndex];
            float dirToPlayerX = Mathf.Sign(player.transform.position.x - currentLeg.transform.position.x);
            float dirToPlayerY = Mathf.Sign(player.transform.position.y - currentLeg.transform.position.y);
            float targetPosX = currentLeg.transform.position.x + stepDst * dirToPlayerX;
            float targetPosY = currentLeg.transform.position.y + stepDst * dirToPlayerY;

            float moveSpeed = .3f;
            Vector3 direction = currentLeg.GetDownwardsDirection();
            Debug.DrawRay(currentLeg.transform.position, direction.normalized * 30, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(currentLeg.transform.position + direction, direction.normalized, 30, groundLayerMask);

            float moveAmount = 30f;
            float counter = 0;

            // move a little in the player's direction
            if (hit)
            {
                while (currentLeg != null && counter < moveAmount && !currentLeg.hit)
                {
                    counter++;
                    direction = currentLeg.GetDownwardsDirection();
                    Debug.DrawRay(currentLeg.transform.position, direction.normalized * 30, Color.red);
                    RaycastHit2D hit2 = Physics2D.Raycast(currentLeg.transform.position + direction, direction.normalized, 30, groundLayerMask);
                    Quaternion angle = Quaternion.FromToRotation(Vector2.up, hit2.normal);

                    currentLeg.transform.Translate(Vector2.right * dirToPlayerX * moveSpeed);
                    currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, angle, 0.2f);

                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                Quaternion downwardAngle = Quaternion.AngleAxis(0, Vector3.forward);

                while (currentLeg != null && currentLeg.transform.rotation != downwardAngle)
                {
                    currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, downwardAngle, Time.deltaTime * 5);
                    yield return new WaitForSeconds(0.01f);
                }
            }

        }
    }

    //IEnumerator RotateToGround(int currentLegIndex)
    //{
    //    Debug.Log("RotateToGround");
    //    if (iceLegs[currentLegIndex] != null && player != null)
    //    {
    //        IceLeg currentLeg = iceLegs[currentLegIndex];
    //        float speed = 10f;
    //        float t = 0f;

    //        Vector3 direction = currentLeg.GetDownwardsDirection();
    //        Debug.DrawRay(currentLeg.transform.position, direction.normalized * 30, Color.red);
    //        RaycastHit2D hit = Physics2D.Raycast(currentLeg.transform.position + direction, direction.normalized, 30, groundLayerMask);
    //        Quaternion angle = Quaternion.FromToRotation(Vector2.up, new Vector3(hit.normal.x, hit.normal.y));

    //        if (hit)
    //        {
    //            while (currentLeg != null && t < 1f)
    //            {
    //                t += speed * Time.deltaTime;
    //                //Debug.Log("leg: " + currentLeg.transform.rotation.eulerAngles.z + " angle: " + angle.eulerAngles.z);
    //                currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, angle, t);
    //                yield return new WaitForSeconds(0.01f);
    //            }
    //        }
    //        else
    //        {
    //            Quaternion downwardAngle = Quaternion.AngleAxis(0, Vector3.forward);

    //            while (currentLeg != null && currentLeg.transform.rotation != downwardAngle)
    //            {
    //                currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, downwardAngle, Time.deltaTime * 5);
    //                yield return new WaitForSeconds(0.01f);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// Points the leg at the player
    /// </summary>
    /// <param name="currentLegIndex">The index of the leg to rotate</param>
    /// <returns></returns>
    IEnumerator PointAtPlayer(int currentLegIndex)
    {
        Debug.Log("PointAtPlayer");
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            Transform currentLeg = iceLegs[currentLegIndex].transform;
            float speed = 20f;
            Vector3 vectorToTarget = currentLeg.position - player.transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion angleToPlayer = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            StopCoroutine("RotateToGround");

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
        Debug.Log("MoveToPlayer");
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
                    if (currentLeg.hit.collider.CompareTag("Snow"))
                    {
                        camShake.Shake(0.05f, 0.05f);
                        hitGround = true;
                        currentLeg.PlayAudio();
                        break;
                    }
                    else if (damagePlayer && currentLeg.hit.collider.CompareTag("Player"))
                    {
                        player.DamagePlayer(damageToPlayerHealth);
                        camShake.Shake(0.08f, 0.08f);
                        damagePlayer = false;
                        currentLeg.PlayAudio(); // this will have to be changed to the player hit sound
                    }
                }
                currentLeg.transform.Translate(Vector2.down * moveSpeed);
                if (!damagePlayer)
                {
                    break;
                }
                if (dstFromHead > legCallbackDst)
                {
                    break;
                }
                dstFromHead = Mathf.Abs((head.transform.position - currentLeg.transform.position).magnitude);

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
        Debug.Log("MoveDown");
        damagePlayer = true;
        if (iceLegs[currentLegIndex] != null && player != null)
        {
            IceLeg currentLeg = iceLegs[currentLegIndex];
            float moveSpeed = 1.5f;
            float dstFromHead = Mathf.Abs((head.transform.position - currentLeg.transform.position).magnitude);

            while (currentLeg != null)
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
                        currentLeg.PlayAudio();
                        camShake.Shake(0.08f, 0.08f);
                        break;
                    }
                }
                currentLeg.transform.Translate(Vector2.down * moveSpeed);

                //currentLeg.transform.position = new Vector3(currentLeg.transform.position.x, currentLeg.transform.position.y - moveSpeed, currentLeg.transform.position.z);
                // This makes sure that if he walks off a cliff he doesn't fall forever
                dstFromHead = Mathf.Abs((head.transform.position - currentLeg.transform.position).magnitude);
                if (dstFromHead > legCallbackDst)
                {
                    break;
                }
                yield return new WaitForSeconds(.01f);
            }

        }
        damagePlayer = false;

    }

    IEnumerator RotateDownwards(int currentLegIndex)
    {
        Debug.Log("RotateDownwards");
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
