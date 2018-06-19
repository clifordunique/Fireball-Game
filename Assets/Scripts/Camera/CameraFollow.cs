using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    public GameObject audioListenerPos;

    public Controller2D target;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookAheadDstY;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;

    public List<Enemy> enemiesToFollow;
    public float followEnemyDst;

    FocusArea focusArea;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;

    float currentLookAheadY;
    float targetLookAheadY;
    float lookAheadDirY;
    float smoothLookVelocityY;
    readonly float smoothVelocityY;

    bool lookAheadStoppedX;
    bool lookAheadStoppedY;

    float shakeX = 0;
    float shakeY = 0;

    void Start()
    {
        focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
    }

    // All player movement has been finished for the frame in its own update method
    void LateUpdate()
    {
        audioListenerPos.transform.position = focusArea.center;

        if (target.collider != null)
        {
            focusArea.Update(target.collider.bounds);
        }

        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        // Check if something is modifying the lookahead
        HorizontalLookahead();
        VerticalLookahead();

        // Horizontal smoothing
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX) + shakeX;

        //Vertical smoothing
        currentLookAheadY = Mathf.SmoothDamp(currentLookAheadY, targetLookAheadY, ref smoothLookVelocityY, verticalSmoothTime) + shakeY;

        focusPosition += Vector2.right * currentLookAheadX;
        focusPosition += Vector2.up * currentLookAheadY;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    private void VerticalLookahead()
    {
        // If the focus area is moving, set it sign appropriately
        lookAheadDirY = Mathf.Sign(focusArea.velocity.y);
        if (Mathf.Sign(target.playerVelocity.y) == Mathf.Sign(focusArea.velocity.y)
            && (target.playerVelocity.y < -0.05f && target.collisions.descendingSlope))
        {
            //lookAheadStoppedY = false;
            targetLookAheadY = lookAheadDirY * lookAheadDstY;
            Debug.Log(target.playerVelocity.y);
        }
        else // it'll just look up
        {
            lookAheadStoppedY = true;
            targetLookAheadY = lookAheadDstY;
        }
    }

    /* This function looks ahead in the x direction that the player is moving in.
     */
    private void HorizontalLookahead()
    {
        // Block stopping the lookahead if the character stops moving
        if (focusArea.velocity.x != 0)
        {
            // If the focus area is moving, set it sign appropriately
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
            {
                lookAheadStoppedX = false;

                if (enemiesToFollow.Count == 0)
                {
                    targetLookAheadX = lookAheadDirX * lookAheadDstX;
                }
                else
                {
                    float dstToEnemy = CalculateCameraPos() + lookAheadDirX * lookAheadDstX;
                    lookAheadDirX = Mathf.Sign(dstToEnemy);
                    targetLookAheadX = lookAheadDirX * Mathf.Abs(dstToEnemy) / (enemiesToFollow.Count + 1);
                }
            }
            else
            {
                if (!lookAheadStoppedX)
                {
                    lookAheadStoppedX = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                }
            }
        }
    }

    float CalculateCameraPos()
    {
        float dst = 0;
        foreach (Enemy e in enemiesToFollow)
        {
            if (e == null)
            {
                enemiesToFollow.Remove(e);
                break;
            }
            dst += e.transform.position.x - focusArea.center.x;
        }

        return dst;
    }

    public void UpdateShake(float _shakeX, float _shakeY)
    {
        shakeX = _shakeX;
        shakeY = _shakeY;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        //Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy.cameraFollow)
            {
                enemiesToFollow.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy.cameraFollow)
            {
                enemiesToFollow.Remove(enemy);
            }
        }
    }
}
