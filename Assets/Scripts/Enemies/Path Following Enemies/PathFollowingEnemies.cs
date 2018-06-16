using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PathFollowingEnemies : Enemy
{
    public float speed = 8;
    public float chaseSpeed = 12;
    public float waitTime = .2f;

    public int damageToPlayerFire = 10;
    public int damageToPlayerHealth = 10;

    public LayerMask playerMask;
    public Transform pathHolder;

    protected float delayedDirToPlayer;

    protected AudioManager audioManager;
    protected PlayerStats stats;
    protected Animator anim;

    public Transform player;

    Collider2D playerCollider;
    Collider2D enemyCollider;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        stats = PlayerStats.instance;
        anim = GetComponent<Animator>();
        audioManager = AudioManager.instance;


        enemyCollider = GetComponent<Collider2D>();

        player.GetComponent<Player>().onFireChangeEvent += OnFireChange;
        playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();

        SetWaypoints();
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

    void OnFireChange(bool isFire) // TODO: change this to access playerstats
    {
        Physics2D.IgnoreCollision(playerCollider, enemyCollider, isFire);
    }

    /* Damages the enemy and destroys it if its health is less than zero
*/
    public override void DamageEnemy(int _damage, Vector2 pos)
    {
        base.DamageEnemy(_damage, pos);
        transform.localScale *= (health + 6 / (health + .1f)) / maxHealth;  // Weird equation for scaling the enemy on hits - maybe make it better

        Effect(pos);

        if (Mathf.Abs(transform.position.x - player.position.x) < seePlayerDistanceX && Mathf.Abs(transform.position.y - player.position.y) < seePlayerDistanceY)
        {
            StopAllCoroutines();
            StartCoroutine(ChasePlayer(false));
        }
        if (health <= 5)
        {
            FindObjectOfType<Player>().onFireChangeEvent -= OnFireChange;
            Destroy(this.gameObject);
        }
    }

    protected virtual void Effect(Vector2 position)
    {
        //TODO: make it so the splash has a forward velocity if the enemy has a forward velocity so that the splash is visible

        // Facing you
        if ((transform.localScale.x > 0 && player.position.x < transform.position.x) || (transform.localScale.x < 0 && player.position.x > transform.position.x))
        {
            // Player is to the left
            if (player.position.x < transform.position.x)
            {
                if (anim.GetFloat("Speed") == 0)
                {
                    //Transform tempWaterSplat = Instantiate(waterSplat, new Vector2(position.x + 2, position.y), Quaternion.Euler(Vector2.right));
                    //tempWaterSplat.localScale = new Vector2(-tempWaterSplat.localScale.x, tempWaterSplat.localScale.y);
                }
                else
                {
                    //Transform tempWaterSplat = Instantiate(waterSplat, position, Quaternion.Euler(Vector2.right));
                    //tempWaterSplat.localScale = new Vector2(-tempWaterSplat.localScale.x, tempWaterSplat.localScale.y);
                }
            }
            else // Player is to the right
            {
                if (anim.GetFloat("Speed") == 0)
                {
                    //Instantiate(waterSplat, new Vector2(position.x - 2, position.y), Quaternion.Euler(Vector2.right));
                }
                else
                {
                    //Instantiate(waterSplat, position, Quaternion.Euler(Vector2.right));
                }
            }
        }
        else // not facing the player
        {
            // Player is to the left
            if (player.position.x < transform.position.x)
            {
                //Instantiate(waterSplat, position, Quaternion.Euler(Vector2.right));
            }
            else // Player is to the right
            {
                //Transform tempWaterSplat = Instantiate(waterSplat, position, Quaternion.Euler(Vector2.right));
                //tempWaterSplat.localScale = new Vector2(-tempWaterSplat.localScale.x, tempWaterSplat.localScale.y);
            }
        }
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
                Debug.DrawRay(transform.position + new Vector3(0,offset,0), new Vector3(dirX * seePlayerDistanceX, 0, 0));
                if (hit && stats.isFire())
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
            if (stats.isFire())
            {
                player.DamageFire((int)(damageToPlayerFire * ((health + 6 / health) / maxHealth)));
                player.DamagePlayer((int)(damageToPlayerHealth * (health + 6 / health) / maxHealth));
                DamageEnemy(1000, transform.position);
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

        while (!CanSeePlayer())
        {
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

    void SetLocalScale(float dirX)
    {
        transform.localScale = new Vector2(-dirX * Mathf.Abs(transform.localScale.x), transform.localScale.y);
    }

    /* Coroutine where the enemy chases the player
     * @param shouldJump - whether or not the player should jump
     */
    public IEnumerator ChasePlayer(bool shouldJump)
    {
        anim.SetFloat("Speed", chaseSpeed);
        float dirToPlayerX = Mathf.Sign(player.position.x - transform.position.x);
        transform.localScale = new Vector2(-dirToPlayerX * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        float maxChaseSpeed = 0.1f;
        float velocityX = 0;
        float accelerationAmount = 0.005f;

        //Wait for the enemy to finish jumping, then chase the player
        if (shouldJump)
        {
            yield return StartCoroutine(Jump());
        }
        StartCoroutine(GetDirectionToPlayer());
        while (player != null)
        {
            if (!stats.isFire())
            {
                StopAllCoroutines();
                SetWaypoints();
            }
            dirToPlayerX = Mathf.Sign(player.position.x - transform.position.x);

            transform.localScale = new Vector2(-dirToPlayerX * Mathf.Abs(transform.localScale.x), transform.localScale.y);

            if (Mathf.Abs(velocityX) < maxChaseSpeed || Mathf.Sign(velocityX) != Mathf.Sign(delayedDirToPlayer))
            {
                velocityX += accelerationAmount * dirToPlayerX;
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

        while (Time.time < jumpSeconds)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + jumpHeight), .09f);
            yield return null;
        }
    }
}
