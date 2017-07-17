/* Name: WaterDropletEnemy
 * Author: John Paul Depew
 * Description: The controller script for the water droplet enemy. This script sets its movement back and forth between waypoints,
 * and if the player is seen, the water droplet attacks.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class WaterDropletEnemy : MonoBehaviour , Enemy {

    public LayerMask mask;
    public float speed = 8;
    public float chaseSpeed = 12;
    public float waitTime = .2f;
    public Transform pathHolder;
    public float viewDistanceX = 8;
    public float viewDistanceY = 3;
    public float maxHealth = 10;

    public Transform waterSplat;
    public Transform waterSplat2;

    Vector2 dirToPlayer;
    Vector2 dirToPlayerOld;

    float health;
    int damage = 10;

    // If an object damages the enemy, it should always have a PlayerWeapon script attached
    PlayerWeapon weapon;
    AudioManager audioManager;

    SpriteRenderer sr;
    Stopwatch sw;

    Transform player;
    Animator anim;
    Collider2D playerCollider;

	void Start () {
        sw = new Stopwatch();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        health = maxHealth;
        audioManager = AudioManager.instance;
        sr = GetComponent<SpriteRenderer>();
        FindObjectOfType<Player>().onFireEvent += OnFire;
        FindObjectOfType<Player>().offFireEvent += OffFire;
        playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();

        SetWaypoints();
	}
	
	void Update () {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (CanSeePlayer())
        {
            StopAllCoroutines();
            StartCoroutine(ChasePlayer());
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
        if (Mathf.Abs(transform.position.x - player.position.x) < viewDistanceX && Mathf.Abs(transform.position.y - player.position.y) < viewDistanceY)
        {
            if ((transform.localScale.x < 0 && (player.position.x > transform.position.x)) || transform.localScale.x > 0 && (player.position.x < transform.position.x))
            {
                if (!Physics.Linecast(transform.position, player.position, mask) && player.GetComponent<Player>().isFire)
                {
                    if (anim.GetFloat("Speed") <= speed)
                    {
                        return true;
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
            if (transform.position.x == targetWaypoint.x)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                anim.SetFloat("Speed", 0);
                yield return new WaitForSeconds(waitTime);

                dirX = Mathf.Sign(targetWaypoint.x - transform.position.x);
                transform.localScale = new Vector2(-dirX * Mathf.Abs(transform.localScale.x), transform.localScale.y);
                //sr.flipX = !transform.GetComponent<SpriteRenderer>().flipX;

                yield return new WaitForSeconds(waitTime);
                anim.SetFloat("Speed", speed);
            }
            yield return null;
        }
    }

    void SetLocalScale(float dirX)
    {
        transform.localScale = new Vector2(-dirX * Mathf.Abs(transform.localScale.x), transform.localScale.y);
    }
    
    IEnumerator ChasePlayer()
    {
        anim.SetFloat("Speed", chaseSpeed);
        float dirToPlayerX = Mathf.Sign(player.position.x - transform.position.x);
        //transform.GetComponent<SpriteRenderer>().flipX = (dirToPlayerX > 0) ? true : false;
        transform.localScale = new Vector2(-dirToPlayerX * Mathf.Abs(transform.localScale.x), transform.localScale.y);

        //Wait for the enemy to finish jumping, then chase the player
        yield return StartCoroutine(Jump());
        StartCoroutine(GetDirectionToPlayer());
        while (transform.position.x != player.position.x)
        {
            if(!player.GetComponent<Player>().isFire)
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
            yield return null;
        }
    }

    /* Gets the players direction with a slight delay
     * This allows you to change direction without the enemy immediately registering it.
     */
    IEnumerator GetDirectionToPlayer()
    {
        while (true)
        {
            dirToPlayer = (player.position - transform.position);
            yield return new WaitForSeconds(.5f);
            dirToPlayerOld = dirToPlayer;
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

    /* NOT NEEDED -- I now have the object detecting what it collided with
     * Detects when a trigger has entered the enemy's collider.
     * If it has a "PlayerWeapon" script attached, it scales the enemy and calls DamageEnemy.
     *
    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("Entered the trigger");
        if (col.gameObject.GetComponent<PlayerWeapon>() != null)
        {
            weapon = col.gameObject.GetComponent<PlayerWeapon>();
            DamageEnemy(weapon.damage);
            Destroy(col.gameObject);
        }
    }*/

    /* Damages the enemy and destroys it if its health is less than zero and instantiates a splash effect
     */
    public void DamageEnemy(int _damage, Vector2 position)
    {
        health -= _damage;
        transform.localScale *= (health + 6 / (health + .1f)) / maxHealth;  // Weird equation for scaling the enemy on hits - maybe make it better
        Effect(position);
        
        if (health <= 0)
        {
            FindObjectOfType<Player>().onFireEvent -= OnFire;
            FindObjectOfType<Player>().offFireEvent -= OffFire;
            Destroy(this.gameObject);
        }
    }

    /* Damages the enemy and destroys it if its health is less than zero
 */
    public void DamageEnemy(int _damage)
    {
        health -= _damage;
        transform.localScale *= (health + 6 / (health + .1f)) / maxHealth;  // Weird equation for scaling the enemy on hits - maybe make it better

        if (health <= 0)
        {
            FindObjectOfType<Player>().onFireEvent -= OnFire;
            FindObjectOfType<Player>().offFireEvent -= OffFire;
            Destroy(this.gameObject);
        }
    }

    void Effect(Vector2 position)
    {
        //TODO: make it so the splash has a forward velocity if the enemy has a forward velocity so that the splash is visible
        audioManager.PlaySound("Spat");
        Vector2 direction = new Vector2(waterSplat.eulerAngles.x,waterSplat.eulerAngles.y);
        Vector2 negDirection = new Vector2(waterSplat.eulerAngles.x, waterSplat.eulerAngles.y);

        // Facing you
        if ((transform.localScale.x > 0 && player.position.x < transform.position.x) || (transform.localScale.x < 0 && player.position.x > transform.position.x))
        {
            // Player is to the left
            if (player.position.x < transform.position.x)
            {
                if (anim.GetFloat("Speed") == 0)
                {
                    Instantiate(waterSplat2, new Vector2(position.x + 2, position.y), Quaternion.Euler(Vector2.right));
                }
                else
                {
                    Instantiate(waterSplat2, position, Quaternion.Euler(Vector2.right));
                    //UnityEngine.Debug.Log("Enemy is facing the player and the player is left");
                    //waterSplat.GetComponent<SpriteRenderer>().flipX = true;
                    //waterSplat.eulerAngles = new Vector2(-1, 1);
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
                    //UnityEngine.Debug.Log("Enemy is facing the player and the player is right");
                    //waterSplat.GetComponent<SpriteRenderer>().flipX = false;
                    //waterSplat.eulerAngles = new Vector2(1, 1);
                }
            }
        }
        else // not facing the player
        {
            // Player is to the left
            if (player.position.x < transform.position.x)
            {
                Instantiate(waterSplat, position, Quaternion.Euler(Vector2.right));
                //UnityEngine.Debug.Log("Enemy is not facing the player and the player is left");
                //waterSplat.GetComponent<SpriteRenderer>().flipX = false;
                //waterSplat.eulerAngles = new Vector2(1, 1);
            }
            else // Player is to the right
            {
                Instantiate(waterSplat2, position, Quaternion.Euler(Vector2.right));
                //UnityEngine.Debug.Log("Enemy is not facing the player and the player is right");
                //waterSplat.GetComponent<SpriteRenderer>().flipX = true;
                //waterSplat.eulerAngles = new Vector2(-1, 1);
            }
        }

        /*
         * 
        //Facing you
        if ((!GetComponent<SpriteRenderer>().flipX && player.position.x < transform.position.x) || (GetComponent<SpriteRenderer>().flipX && player.position.x > transform.position.x))
        {
            // Player is to the left
            if(player.position.x < transform.position.x)
            {
                if(anim.GetFloat("Speed") == 0)
                {
                    Instantiate(waterSplat2, new Vector2(position.x + 2, position.y), Quaternion.Euler(Vector2.right));
                }
                else
                {
                    Instantiate(waterSplat2, position, Quaternion.Euler(Vector2.right));
                    UnityEngine.Debug.Log("Enemy is facing the player and the player is left");
                    //waterSplat.GetComponent<SpriteRenderer>().flipX = true;
                    //waterSplat.eulerAngles = new Vector2(-1, 1);
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
                    UnityEngine.Debug.Log("Enemy is facing the player and the player is right");
                    //waterSplat.GetComponent<SpriteRenderer>().flipX = false;
                    //waterSplat.eulerAngles = new Vector2(1, 1);
                }
            }
        }
        else // not facing the player
        {
            // Player is to the left
            if (player.position.x < transform.position.x)
            {
                Instantiate(waterSplat, position, Quaternion.Euler(Vector2.right));
                UnityEngine.Debug.Log("Enemy is not facing the player and the player is left");
                //waterSplat.GetComponent<SpriteRenderer>().flipX = false;
                //waterSplat.eulerAngles = new Vector2(1, 1);
            }
            else // Player is to the right
            {
                Instantiate(waterSplat2, position, Quaternion.Euler(Vector2.right));
                UnityEngine.Debug.Log("Enemy is not facing the player and the player is right");
                //waterSplat.GetComponent<SpriteRenderer>().flipX = true;
                //waterSplat.eulerAngles = new Vector2(-1, 1);
            }
        }
        */
    }

    void OnFire()
    {
            Physics2D.IgnoreCollision(playerCollider, GetComponent<Collider2D>(), false);
    }

    void OffFire()
    {
            Physics2D.IgnoreCollision(playerCollider, GetComponent<Collider2D>(), true);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (player.isFire)
            {
                player.DamageFire((int)(damage * ((health + 6 / health) / maxHealth)));
                audioManager.PlaySound("Water Hiss Short");
                DamageEnemy(1000);
            }
        }
        else if(col.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }
    }
}