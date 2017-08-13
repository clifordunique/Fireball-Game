using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour, ISmashable {

    public Transform explodePrefab;

    public void DestroyObject()
    {
        Instantiate(explodePrefab, new Vector2(transform.position.x, transform.position.y + 1), transform.rotation);
        Destroy(gameObject);
    }
}
