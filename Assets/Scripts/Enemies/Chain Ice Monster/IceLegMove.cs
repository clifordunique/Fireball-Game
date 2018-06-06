using System.Collections;
using UnityEngine;

public class IceLegMove : MonoBehaviour
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

    CameraShake camShake;

    void Start()
    {
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        StartCoroutine(Walk());
    }

    IEnumerator Walk()
    {
        while (true)
        {
            for (int i = 0; i < iceLegs.Length; i++)
            {
                if (iceLegs[i].health > 0)
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
            yield return null;
        }
    }

    IEnumerator MoveUp(int currentLegIndex)
    {
        Transform currentLeg = iceLegs[currentLegIndex].transform;
        float targetPosY = currentLeg.position.y + moveUpAmount;
        float moveSpeed = 0.3f;

        Debug.Log("Moving up");

        StartCoroutine("RotateDownwards", currentLegIndex);

        while (currentLeg.position.y < targetPosY)
        {
            if (iceLegs[currentLegIndex] != null) // add this check in case the ice leg was destroyed in the middle of it's coroutine
            {
                currentLeg.position = new Vector3(currentLeg.position.x, currentLeg.position.y + moveSpeed, currentLeg.position.z);
                yield return null;
            }
        }
    }

    IEnumerator MoveToPos(int currentLegIndex)
    {
        Transform currentLeg = iceLegs[currentLegIndex].transform;
        float dirToPlayer = Mathf.Sign(player.transform.position.x - currentLeg.position.x);
        float targetPosX = currentLeg.position.x + stepDst * dirToPlayer;
        float moveSpeed = 0.3f;

        Debug.Log("Moving to position");

        while (currentLeg.position.x * dirToPlayer < targetPosX * dirToPlayer)
        {
            if (iceLegs[currentLegIndex] != null)
            {
                currentLeg.position = new Vector3(currentLeg.position.x + moveSpeed * dirToPlayer, currentLeg.position.y, currentLeg.position.z);
                yield return null;
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
        float speed = 20f;
        Transform currentLeg = iceLegs[currentLegIndex].transform;
        Vector3 vectorToTarget = currentLeg.position - player.transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion angleToPlayer = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        StopCoroutine("RotateDownwards");

        Debug.Log("Rotating to player");

        if (iceLegs[currentLegIndex] != null)
        {
            while (currentLeg.transform.rotation.eulerAngles.z > angleToPlayer.eulerAngles.z + 0.1f || currentLeg.transform.rotation.eulerAngles.z < angleToPlayer.eulerAngles.z - 0.1f)
            {
                if (iceLegs[currentLegIndex] != null)
                {
                    Debug.Log("Still rotating " + currentLeg.transform.rotation + " " + angleToPlayer);
                    currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, angleToPlayer, Time.deltaTime * speed);
                    yield return null;
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
        Transform currentLeg = iceLegs[currentLegIndex].transform;
        Vector2 dirToPlayer = player.transform.position - currentLeg.position;
        dirToPlayer.Normalize();
        float dstFromHead = Mathf.Abs((head.transform.position - currentLeg.transform.position).magnitude);
        float moveSpeed = 1.4f;
        bool hitGround = false;

        Debug.Log("Moving to Player");

        while (!hitGround)
        {
            if (iceLegs[currentLegIndex] != null)
            {
                if (iceLegs[currentLegIndex].hit)
                {
                    camShake.Shake(0.08f, 0.08f);
                    hitGround = true;
                    break;
                }
                currentLeg.Translate(Vector2.down * moveSpeed);
                if (dstFromHead > legCallbackDst)
                {
                    break;
                }
                dstFromHead = Mathf.Abs((head.transform.position - currentLeg.transform.position).magnitude);

                yield return null;
            }
        }

    }

    /// <summary>
    /// Moves the leg at currentLegIndex down until it detects a collision with an object
    /// </summary>
    /// <param name="currentLegIndex">The index of the leg to move down</param>
    /// <returns></returns>
    IEnumerator MoveDown(int currentLegIndex)
    {
        IceLeg currentLeg = iceLegs[currentLegIndex];
        float moveSpeed = 1.4f;
        bool hitGround = false;

        Debug.Log("Moving down");

        while (!hitGround)
        {
            if (currentLeg != null)
            {
                if (currentLeg.hit)
                {
                    camShake.Shake(0.08f, 0.08f);
                    hitGround = true;
                    break;
                }
                currentLeg.transform.position = new Vector3(currentLeg.transform.position.x, currentLeg.transform.position.y - moveSpeed, currentLeg.transform.position.z);

                yield return null;
            }
        }
    }

    IEnumerator RotateDownwards(int currentLegIndex)
    {
        Transform currentLeg = iceLegs[currentLegIndex].transform;
        Quaternion downwardAngle = Quaternion.AngleAxis(0, Vector3.forward);

        Debug.Log("Rotating down");

        if (iceLegs[currentLegIndex] != null)
        {
            while (currentLeg.transform.rotation != downwardAngle)
            {
                Debug.Log("Still rotating downwards");
                if (iceLegs[currentLegIndex] != null)
                {
                    currentLeg.transform.rotation = Quaternion.Slerp(currentLeg.transform.rotation, downwardAngle, Time.deltaTime * 5);
                    yield return null;
                }
            }
        }
    }
}
