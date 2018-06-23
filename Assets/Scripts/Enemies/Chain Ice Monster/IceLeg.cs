using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class IceLeg : Enemy
{
    public Transform metalBrace;
    public Transform tipOfIce;
    public SpriteMask spriteMask;

    public RaycastHit2D hit;
    public float rayStartOffset;
    public float rayLength = 0.5f;
    public LayerMask layerMask;

    AudioSource audioSource;

    private void LateUpdate()
    {
        Vector3 direction = GetDownwardsDirection();
        Debug.DrawRay(transform.position + direction * rayStartOffset, direction.normalized * rayLength, Color.red);
        hit = Physics2D.Raycast(transform.position + direction * rayStartOffset, direction.normalized, rayLength, layerMask);
        audioSource = GetComponent<AudioSource>();
    }

    public override void DamageEnemy(int _damage, Vector2 position)
    {
        health -= _damage;
        if (health <= 0)
        {
            UpdateRigidbody();

            Destroy(this.gameObject);
        }
        Effect(position);
    }

    public Vector3 GetDownwardsDirection()
    {
        return (tipOfIce.position - transform.position).normalized;
    }

    /// <summary>
    /// Creates a raycast from the leg to the ground
    /// </summary>
    /// <returns>Returns the hit</returns>
    //public Quaternion RaycastGroundDetector()
    //{
    //    Vector3 direction = GetDownwardsDirection();
    //    Debug.DrawRay(transform.position + direction * rayStartOffset, direction.normalized * 50, Color.red);
    //    RaycastHit2D hit = Physics2D.Raycast(transform.position + direction * rayStartOffset, direction.normalized, 50, groundLayerMask);

    //        Quaternion angle = Quaternion.FromToRotation(Vector2.up, new Vector3(hit.normal.x, hit.normal.y));
    //        Debug.Log(direction + " " + hit.normal);

    //        return angle;

    //}

    /// <summary>
    /// Creates effect for damaging enemy
    /// </summary>
    /// <param name="hitPos">The position of the hit on the enemy</param>
    void Effect(Vector2 hitPos)
    {
        float dirToHit = Mathf.Sign(hitPos.x - transform.position.x);
        float offset = dirToHit > 0 ? -0.2f : 0.2f;
        Vector3 angle = dirToHit > 0 ? -transform.rotation.eulerAngles : transform.rotation.eulerAngles;
        SpriteMask newSpriteMask = Instantiate(spriteMask, new Vector2(hitPos.x + offset, hitPos.y), Quaternion.Euler(Vector3.forward));
        newSpriteMask.transform.SetParent(transform);
    }

    public void UpdateRigidbody()
    {
        Rigidbody2D metalRb2D = metalBrace.GetComponent<Rigidbody2D>();
        metalBrace.parent = this.transform.parent;
        metalBrace.GetComponent<BoxCollider2D>().enabled = true;

        metalRb2D.bodyType = RigidbodyType2D.Dynamic;
        metalRb2D.mass = 50;
        metalRb2D.gravityScale = 5;
    }

    public void PlayAudio()
    {
        audioSource.Play();
    }
}