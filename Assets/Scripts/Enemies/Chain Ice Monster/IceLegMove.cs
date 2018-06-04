using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceLegMove : MonoBehaviour
{
    public IceLeg[] iceLegs;
    //public Transform[] iceLegs;
    public float moveUpAmount = 5f;
    public float stepDst = 20f;
    public float crippleWaitTime = 0.4f;
    public Player player;

    public Vector3 rayStartPos;
    public float rayLength = 0.5f;
    public LayerMask layerMask;

    CameraShake camShake;

    void Start()
    {
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        StartCoroutine(Walk());
    }

    private void Update()
    {

    }

    IEnumerator Walk()
    {
        while (true)
        {
            for (int i = 0; i < iceLegs.Length; i++)
            {
                if (iceLegs[i].health > 0)
                {
                    yield return MoveUp(i);
                    yield return MoveToPos(i);
                    yield return MoveDown(i);
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
    /// Moves the leg at currentLegIndex down until it detects a collision with an object
    /// </summary>
    /// <param name="currentLegIndex">The index of the leg to move down</param>
    /// <returns></returns>
    IEnumerator MoveDown(int currentLegIndex)
    {
        Transform currentLeg = iceLegs[currentLegIndex].transform;
        float moveSpeed = 1.4f;
        bool hitGround = false;

        // Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

        while (!hitGround)
        {
            if (iceLegs[currentLegIndex] != null)
            {
                RaycastHit2D hit = Physics2D.Raycast(currentLeg.position + rayStartPos, Vector2.down, rayLength, layerMask);
                Debug.DrawRay(currentLeg.position + rayStartPos, Vector2.down * rayLength, Color.red);
                if (hit)
                {
                    camShake.Shake(0.08f, 0.08f);
                    hitGround = true;
                    break;
                }
                currentLeg.position = new Vector3(currentLeg.position.x, currentLeg.position.y - moveSpeed, currentLeg.position.z);

                yield return null;
            }
        }
    }
}
