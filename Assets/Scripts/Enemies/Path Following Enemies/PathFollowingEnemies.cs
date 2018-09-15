using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class PathFollowingEnemies : Enemy
{
    public float speed = 8;
    public float chaseSpeed = 10;
    public float accelerationSpeed = 5;
    public float waitTime = .2f;

    public LayerMask playerMask;
    public Transform pathHolder;

    public Utilities.PlatformType platformType;
    private string platformSoundName;

    protected float delayedDirToPlayer;
    private bool isPlayerFire;

    protected AudioManager audioManager;
    protected PlayerStats stats;
    protected Animator anim;
    protected AudioSource audioSource;

    Transform player;

    Collider2D playerCollider;
    Collider2D enemyCollider;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        stats = PlayerStats.instance;
        anim = GetComponent<Animator>();
        audioManager = AudioManager.instance;
        audioSource = GetComponent<AudioSource>();
        player = FindObjectOfType<Player>().transform;

        playerCollider = player.GetComponent<Collider2D>();
        enemyCollider = GetComponent<Collider2D>();

        switch (platformType)
        {
            case Utilities.PlatformType.GRASS:
                platformSoundName = "Grass02";
                break;
            default:
                break;
        }

        //player.GetComponent<Player>().onFireChangeEvent += OnFireChange;
        //playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();
        ToggleIgnorePlayer();
        isPlayerFire = stats.IsFire();
        SetWaypoints();
    }

    protected virtual void Update()
    {
        if (isPlayerFire != stats.IsFire())
        {
            ToggleIgnorePlayer();
        }
    }

    private void SetWaypoints()
    {
        Vector2[] waypoints = new Vector2[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        StartCoroutine(FollowPath(waypoints));
    }

    /* Damages the enemy and destroys it if its health is less than zero
*/
    public override void DamageEnemy(int _damage, Vector2 pos)
    {
        base.DamageEnemy(_damage, pos);

        Effect(pos);

        StopAllCoroutines();
        StartCoroutine(ChasePlayer(false));

        if (health <= 5)
        {
            //FindObjectOfType<Player>().onFireChangeEvent -= OnFireChange;
            Destroy(this.transform.parent.gameObject);
        }
    }

    protected virtual void Effect(Vector2 position)
    {

    }

    /* Tests whether the enemy can see the player or not
 * @returns true if the enemy can see the player
 */
    // appears to not be working, but it just because the player is too short... add more raycasts
    protected bool CanSeePlayer()
    {
        float dirX = -Mathf.Sign(transform.localScale.x);
        if (player != null)
        {
            int horizontalRayCount = 5;
            float offset = 0.5f;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + offset), Vector2.right * dirX, seePlayerDistanceX, playerMask);
                Debug.DrawRay(transform.position + new Vector3(0, offset, 0), new Vector3(dirX * seePlayerDistanceX, 0, 0));
                if (hit && hit.collider.CompareTag("Player") && stats.IsFire())
                {
                    return true;
                }
                offset -= 0.5f;
            }
        }
        return false;
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (stats.IsFire())
            {
                int healthDamage = (int)(damageToPlayerHealth * (health + 6 / health) / maxHealth);
                int fireDamage = (int)(damageToPlayerFire * ((health + 6 / health) / maxHealth));
                damagePlayerData.damageToPlayerHealth = healthDamage;
                damagePlayerData.damageToPlayerFireHealth = fireDamage;

                player.DamagePlayer(damagePlayerData);
                DamageEnemy(1000, player.transform.position);
            }
        }
        else if (col.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }
    }

    /* Follows a path along an array of waypoints. Once it has reached the last waypoint, it goes back to the first.
 * It makes sure the Enemy is facing the way it is walking.
 */
    IEnumerator FollowPath(Vector2[] waypoints)
    {
        int targetWaypointIndex = 1;
        Vector2 targetWaypoint = waypoints[targetWaypointIndex];
        float dirX;
        float waitTime = 0.4f;
        float targetTime = Time.time + waitTime;

        while (!CanSeePlayer())
        {
            if (Time.time > targetTime)
            {
                audioSource.Play();
                targetTime = Time.time + waitTime;
            }
            dirX = Mathf.Sign(targetWaypoint.x - transform.position.x);
            transform.localScale = new Vector2(-dirX * Mathf.Abs(transform.localScale.x), transform.localScale.y);
            anim.SetFloat("Speed", speed);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetWaypoint.x, transform.position.y), speed * Time.deltaTime);
            if (Math.Round(transform.position.x, 2) == Math.Round(targetWaypoint.x, 2))
            {

                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                anim.SetFloat("Speed", 0);
                yield return new WaitForSeconds(waitTime);

                dirX = Mathf.Sign(targetWaypoint.x - transform.position.x);
                transform.localScale = new Vector2(-dirX * Mathf.Abs(transform.localScale.x), transform.localScale.y);

                yield return new WaitForSeconds(waitTime);
                anim.SetFloat("Speed", speed);
            }

            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine(ChasePlayer(true));
    }

    /* Coroutine where the enemy chases the player
     * @param shouldJump - whether or not the player should jump
     */
    public IEnumerator ChasePlayer(bool shouldJump)
    {
        float dirToPlayerX = Mathf.Sign(player.position.x - transform.position.x);
        transform.localScale = new Vector2(-dirToPlayerX * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        float velocityX = 0;
        float waitTime = 0.25f;
        float targetTime = Time.time + waitTime;

        //Wait for the enemy to finish jumping, then chase the player
        if (shouldJump)
        {
            yield return Jump();
        }
        StartCoroutine(GetDirectionToPlayer());
        anim.SetFloat("Speed", chaseSpeed);
        while (player != null)
        {
            if (Time.time > targetTime)
            {
                audioSource.Play();
                targetTime = Time.time + waitTime;
            }
            if (!stats.IsFire())
            {
                ToggleIgnorePlayer();
                StopAllCoroutines();
                SetWaypoints();
            }
            dirToPlayerX = Mathf.Sign(player.position.x - transform.position.x);

            transform.localScale = new Vector2(-dirToPlayerX * Mathf.Abs(transform.localScale.x), transform.localScale.y);

            if (Mathf.Abs(velocityX) < chaseSpeed / 100 || Mathf.Sign(velocityX) != Mathf.Sign(delayedDirToPlayer))
            {
                velocityX += (accelerationSpeed / 1000) * dirToPlayerX;
            }

            transform.Translate(new Vector2(velocityX, 0));
            yield return new WaitForSeconds(0.01f);
        }
    }

    /* Gets the players direction with a slight delay
     * This allows you to change direction without the enemy immediately registering it.
     */
    IEnumerator GetDirectionToPlayer()
    {
        while (player != null && true)
        {
            delayedDirToPlayer = (player.position.x - transform.position.x);
            yield return new WaitForSeconds(1f);
        }
    }

    /* Makes the enemy jump into the air
     */
    IEnumerator Jump()
    {
        float jumpHeight = 2;
        float jumpSeconds = Time.time + 0.3f;
        anim.SetFloat("Speed", chaseSpeed);
        while (Time.time < jumpSeconds)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + jumpHeight), .09f);
            yield return new WaitForSeconds(0.01f);
        }
        jumpSeconds = Time.time + 0.4f;
        while (Time.time < jumpSeconds)
        {
            yield return new WaitForSeconds(0.01f);
        }
    }

    void ToggleIgnorePlayer()
    {
        Physics2D.IgnoreCollision(playerCollider, enemyCollider, !stats.IsFire());
    }
}
