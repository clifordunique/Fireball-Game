using System.Collections;
using UnityEngine;

public class ObjectDestroy : MonoBehaviour
{
    public float timeToDestroy = 2f;

    private float targetTime = 0;

    private void Start()
    {
        targetTime = Time.time + timeToDestroy;
    }

    private void Update()
    {
        if(targetTime < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
