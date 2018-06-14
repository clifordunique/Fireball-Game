/* Name: WaterDropletEnemy
 * Author: John Paul Depew
 * Description: The controller script for the water droplet enemy. This script sets its movement back and forth between waypoints,
 * and if the player is seen, the water droplet attacks.
 */

using System.Collections;
using UnityEngine;
using System.Diagnostics;
using System;

public class WaterDropletEnemy : Enemy
{
    public LayerMask mask;
    public float speed = 8;
    public float chaseSpeed = 12;
    public float waitTime = .2f;
    public Transform pathHolder;

    public Transform waterSplat;

    Vector2 dirToPlayer;

    int damage = 10;
    bool canSeePlayer = false;

    AudioManager audioManager;
    PlayerStats stats;

    public Transform player;
    Animator anim;
    Collider2D playerCollider;
    Collider2D enemyCollider;

    public override void Start()
    {
        base.Start();
        stats = PlayerStats.instance;
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        audioManager = AudioManager.instance;
        player.GetComponent<Player>().onFireChangeEvent += OnFireChange;
        playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();

        SetWaypoints();
    }

    void Update()
    {
        // First check and see if the state is paused. If it is, return.
        if (GameMaster.gm.CurState == Utilities.State.PAUSED)
        {
            return;
        }

        if (!canSeePlayer)
        {
            canSeePlayer = CanSeePlayer();
        }
        if (canSeePlayer)
        {
            StopAllCoroutines();
            StartCoroutine(ChasePlayer(true));
            canSeePlayer = false; // Now it won't start a bunch of coroutines
        }
    }

    void SetWaypoints()
    {
        Vector2[] waypoints = new Vector2[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        StartCoroutine(FollowPath(waypoints));
    }

    /* Tests whether the enemy can see the player or not
     * @returns true if the enemy can see the player
     */
    bool CanSeePlayer()
    {
        if (player != null)
        {
            if (Mathf.Abs(transform.position.x - player.position.x) < seePlayerDistanceX && Mathf.Abs(transform.position.y - player.position.y) < seePlayerDistanceY)
            {
                // If the player is in front of the water droplet
                if ((transform.localScale.x < 0 && (player.position.x > transform.position.x)) || transform.localScale.x > 0 && (player.position.x < transform.position.x))
                {
                    if (!Physics.Linecast(transform.position, player.position, mask) && stats.isFire())
                    {
                        if (anim.GetFloat("Speed") <= speed)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    /* Follows a path along an array of waypoints. Once it has reached the last waypoint, it goes back to the first.
     * It makes sure the Enemy is facing the way it is walking.
     */
    IEnumerator FollowPath(Vector2[] waypoints)
    {
        int targetWaypointIndex = 1;
        Vector2 targetWaypoint = waypoints[targetWaypointIndex];
        float dirX;

        while (true)
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
    }

    void SetLocalScale(float dirX)
    {
        transform.localScale = new Vector2(-dirX * Mathf.Abs(transform.localScale.x), transform.localScale.y);
    }

    /* Coroutine where the enemy chases the player
     * @param shouldJump - whether or not the player should jump
     */
    IEnumerator ChasePlayer(bool shouldJump)
    {
        anim.SetFloat("Speed", chaseSpeed);
        float dirToPlayerX = Mathf.Sign(player.position.x - transform.position.x);
        //transform.GetComponent<SpriteRenderer>().flipX = (dirToPlayerX > 0) ? true : false;
        transform.localScale = new Vector2(-dirToPlayerX * Mathf.Abs(transform.localScale.x), transform.localScale.y);

        //Wait for the enemy to finish jumping, then chase the player
        if (shouldJump)
        {
            yield return StartCoroutine(Jump());
        }
        StartCoroutine(GetDirectionToPlayer());
        while (player != null && transform.position.x != player.position.x)
        {
            if (!stats.isFire())
            {
                StopAllCoroutines();
                SetWaypoints();
            }
            dirToPlayerX = Mathf.Sign(player.position.x - transform.position.x);
            //transform.GetComponent<SpriteRenderer>().flipX = (dirToPlayerX > 0) ? true : false;
            transform.localScale = new Vector2(-dirToPlayerX * Mathf.Abs(transform.localScale.x), transform.localScale.y);
            //transform.Translate(dirToPlayerX * Time.deltaTime * chaseSpeed);
            //transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), chaseSpeed * Time.deltaTime);
            if (dirToPlayer.y < 0)
            {
                dirToPlayer.y = 0;
            }
            transform.Translate(dirToPlayer.normalized * chaseSpeed / 50);
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
            dirToPlayer = (player.position - transform.position);
            yield return new WaitForSeconds(.5f);
        }
    }

    /* Makes the enemy jump into the air
     */
    IEnumerator Jump()
    {
        Stopwatch sw = new Stopwatch();
        float jumpHeight = 2;

        sw.Start();
        while (sw.ElapsedMilliseconds < 500)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + jumpHeight), .2f);
            yield return null;
        }
    }

    /* Damages the enemy and destroys it if its health is less than zero
    */
    public override void DamageEnemy(int _damage, Vector2 pos)
    {
        health -= _damage;
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

    public void Effect(Vector2 position)
    {
        //TODO: make it so the splash has a forward velocity if the enemy has a forward velocity so that the splash is visible
        audioManager.PlaySound("Spat");
        audioManager.PlaySound("Water Hiss Short");

        // Facing you
        if ((transform.localScale.x > 0 && player.position.x < transform.position.x) || (transform.localScale.x < 0 && player.position.x > transform.position.x))
        {
            // Player is to the left
            if (player.position.x < transform.position.x)
            {
                if (anim.GetFloat("Speed") == 0)
                {
                    Transform tempWaterSplat = Instantiate(waterSplat, new Vector2(position.x + 2, position.y), Quaternion.Euler(Vector2.right));
                    tempWaterSplat.localScale = new Vector2(-tempWaterSplat.localScale.x, tempWaterSplat.localScale.y);
                }
                else
                {
                    Transform tempWaterSplat = Instantiate(waterSplat, position, Quaternion.Euler(Vector2.right));
                    tempWaterSplat.localScale = new Vector2(-tempWaterSplat.localScale.x, tempWaterSplat.localScale.y);
                }
            }
            else // Player is to the right
            {
                if (anim.GetFloat("Speed") == 0)
                {
                    Instantiate(waterSplat, new Vector2(position.x - 2, position.y), Quaternion.Euler(Vector2.right));
                }
                else
                {
                    Instantiate(waterSplat, position, Quaternion.Euler(Vector2.right));
                }
            }
        }
        else // not facing the player
        {
            // Player is to the left
            if (player.position.x < transform.position.x)
            {
                Instantiate(waterSplat, position, Quaternion.Euler(Vector2.right));
            }
            else // Player is to the right
            {
                Transform tempWaterSplat = Instantiate(waterSplat, position, Quaternion.Euler(Vector2.right));
                tempWaterSplat.localScale = new Vector2(-tempWaterSplat.localScale.x, tempWaterSplat.localScale.y);
            }
        }
    }

    void OnFireChange(bool isFire) // TODO: change this to access playerstats
    {
        Physics2D.IgnoreCollision(playerCollider, enemyCollider, isFire);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (stats.isFire())
            {
                player.DamageFire((int)(damage * ((health + 6 / health) / maxHealth)));
                audioManager.PlaySound("Water Hiss Short");
                DamageEnemy(1000, transform.position);
            }
        }
        else if (col.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }
    }
}