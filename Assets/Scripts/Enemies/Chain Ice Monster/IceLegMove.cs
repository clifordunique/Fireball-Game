﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceLegMove : MonoBehaviour
{
    public Transform[] iceLegs;
    public float moveUpAmount = 5f;
    public float stepDst = 20f;
    public Player player;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Walk());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Walk()
    {
        while (true)
        {
            for (int i = 0; i < iceLegs.Length; i++)
            {
                yield return MoveUp(i);
                yield return MoveToPos(i);
                yield return MoveDown(i);
            }
            yield return null;
        }
    }

    IEnumerator MoveUp(int currentLegIndex)
    {
        Transform currentLeg = iceLegs[currentLegIndex];
        float targetPosY = currentLeg.position.y + moveUpAmount;
        float moveSpeed = 0.5f;

        while (currentLeg.position.y < targetPosY)
        {
            currentLeg.position = new Vector3(currentLeg.position.x, currentLeg.position.y + moveSpeed, currentLeg.position.z);
            yield return null;
        }
    }

    IEnumerator MoveToPos(int currentLegIndex)
    {
        Transform currentLeg = iceLegs[currentLegIndex];
        float dirToPlayer = Mathf.Sign(player.transform.position.x - currentLeg.position.x);
        float targetPosX = currentLeg.position.x + stepDst * dirToPlayer;
        float moveSpeed = 0.5f;

        while (currentLeg.position.x * dirToPlayer < targetPosX * dirToPlayer)
        {
            currentLeg.position = new Vector3(currentLeg.position.x + moveSpeed * dirToPlayer, currentLeg.position.y, currentLeg.position.z);
            yield return null;
        }
    }

    IEnumerator MoveDown(int currentLegIndex)
    {
        Transform currentLeg = iceLegs[currentLegIndex];
        float targetPosY = currentLeg.position.y - moveUpAmount;
        float moveSpeed = 0.5f;

        while (currentLeg.position.y >= targetPosY)
        {
            currentLeg.position = new Vector3(currentLeg.position.x, currentLeg.position.y - moveSpeed, currentLeg.position.z);
            yield return null;
        }
    }
}
