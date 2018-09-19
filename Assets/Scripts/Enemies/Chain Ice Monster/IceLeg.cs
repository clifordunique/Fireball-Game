using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class IceLeg : Enemy
{
    public Transform metalBrace;
    public Transform tipOfIce;
    public Transform leftSide;
    public Transform rightSide;
    public SpriteMask spriteMask;

    public float rayStartOffset;
    public float rayLength = 0.5f;
    public const float sideRayLength = 5f;
    private RaycastHit2D hit;
    private RaycastHit2D groundDetector;

    AudioSource audioSource;

    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
    }

    public override void DamageEnemy(int _damage, Vector2 position)
    {
        health -= _damage;
        if (health <= 0)
        {
            UpdateRigidbody();
            base.DamageEnemy(_damage, position);
        }
        Effect(position);
    }

    public Vector3 GetDownwardsDirection()
    {
        return (tipOfIce.position - transform.position).normalized;
    }

    /// <summary>
    /// Gets the short hit from the leg for detecting impacts
    /// </summary>
    /// <param name="mask">The layermask for the ray to pay attention to</param>
    /// <returns>The hit info</returns>
    public RaycastHit2D GetHit(LayerMask mask)
    {
        Vector3 direction = GetDownwardsDirection();
        Debug.DrawRay(transform.position + direction * rayStartOffset, direction.normalized * rayLength, Color.blue);
        hit = Physics2D.Raycast(transform.position + direction * rayStartOffset, direction.normalized, rayLength, mask);
        return hit;
    }

    /// <summary>
    /// Gets the longer hit from the leg for detecting info about the ground
    /// </summary>
    /// <param name="mask">The layermask for the ray to pay attention to</param>
    /// <returns>The hit info</returns>
    public RaycastHit2D GetGroundDetectorHit(LayerMask mask)
    {
        Vector3 direction = GetDownwardsDirection();
        Debug.DrawRay(transform.position, direction.normalized * 30, Color.red);
        groundDetector = Physics2D.Raycast(transform.position + direction, direction.normalized, 30, mask);
        return groundDetector;
    }

    /// <summary>
    /// This casts a ray to the side of the leg, either projecting it out, or just the width of the leg
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="direction">The direction to cast the ray</param>
    /// <param name="longRayLength">Whether or not to cast a long ray to the side or just one that spans the width of the leg</param>
    /// <returns></returns>
    public RaycastHit2D GetSideHit(LayerMask mask, float direction, bool longRayLength)
    {
        Vector2 directionBtwSides = rightSide.position - leftSide.position;
        float dstBetweenSides = directionBtwSides.magnitude;
        directionBtwSides.Normalize();
        float actualRayLength = longRayLength ? sideRayLength : dstBetweenSides;
        direction = longRayLength ? direction : -1;

        RaycastHit2D sideHit = Physics2D.Raycast(rightSide.position, direction * directionBtwSides, actualRayLength, mask);

        Debug.DrawRay(rightSide.position, directionBtwSides * direction * actualRayLength, Color.red);

        return sideHit;
    }

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