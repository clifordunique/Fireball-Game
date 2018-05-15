using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    public float speed = 30f;

    void Start()
    {
        StartCoroutine(ResizeCollider());
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        Destroy(gameObject, 3f);
    }

    IEnumerator ResizeCollider()
    {
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        yield return new WaitForSeconds(.1f);
        boxCollider2D.size = new Vector2(boxCollider2D.size.x, 6);

    }
}
