using UnityEngine;
using System;

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
    private bool modLooking = false;
    public bool ModLooking
    {
        get { return modLooking; }
        set { modLooking = value; }
    }

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
        if (!modLooking)
        {
            HorizontalLookahead();
        }
        else
        {
            HorizontalLookaheadMod();
        }
        VerticalLookahead();

        // Horizontal smoothing
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX) + shakeX;

        //Vertical smoothing
        currentLookAheadY = Mathf.SmoothDamp(currentLookAheadY, targetLookAheadY, ref smoothLookVelocityY, verticalSmoothTime) + shakeY;
        //focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        //Debug.Log("target: " + targetLookAheadY + " current: " + currentLookAheadY + " focus: " + focusPosition);

        focusPosition += Vector2.right * currentLookAheadX;
        focusPosition += Vector2.up * currentLookAheadY;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    private void VerticalLookahead()
    {
        if (focusArea.velocity.y != 0)
        {
            // If the focus area is moving, set it sign appropriately
            lookAheadDirY = Mathf.Sign(focusArea.velocity.y);
            if (Mathf.Sign(target.playerVelocity.y) == Mathf.Sign(focusArea.velocity.y)
                && (target.playerVelocity.y > .01f || (target.playerVelocity.y < -.01f && target.collisions.descendingSlope)))
            {
                lookAheadStoppedY = false;
                targetLookAheadY = lookAheadDirY * lookAheadDstY;
            }
            else
            {
                if (!lookAheadStoppedY)
                {
                    lookAheadStoppedY = true;
                    targetLookAheadY = currentLookAheadY + (lookAheadDirY * lookAheadDstY - currentLookAheadY) / 4f;
                }
            }
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
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
                //if (target.playerVelocity.y > -0.01f)
                //{
                //    targetLookAheadY = currentLookAheadY + (lookAheadDstY - currentLookAheadY) / 4f;
                //}
            }
            else
            {
                if (!lookAheadStoppedX)
                {
                    lookAheadStoppedX = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;

                    // NOT FULLY FUNCTIONAL CODE:
                    //targetLookAheadY = currentLookAheadY + (lookAheadDirY * lookAheadDstY - currentLookAheadY) / 4f;
                }
            }

            //if (!(target.playerVelocity.y < -0.1f))
            //{
            //    lookAheadStoppedY = false;
            //    targetLookAheadY = lookAheadDstY;
            //}
        }
    }

    /* This function looks ahead in the positive direction x
    */
    private void HorizontalLookaheadMod()
    {
        // Block stopping the lookahead if the character stops moving
        if (focusArea.velocity.x != 0)
        {
            // If the focus area is moving, set it sign appropriately
            lookAheadDirX = Math.Abs(Mathf.Sign(focusArea.velocity.x));
            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
            {
                lookAheadStoppedX = false;
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
                //if (target.playerVelocity.y > -0.01f)
                //{
                //    targetLookAheadY = currentLookAheadY + (lookAheadDstY - currentLookAheadY) / 4f;
                //}
            }
            else
            {
                if (!lookAheadStoppedX)
                {
                    lookAheadStoppedX = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;

                    // NOT FULLY FUNCTIONAL CODE:
                    //targetLookAheadY = currentLookAheadY + (lookAheadDirY * lookAheadDstY - currentLookAheadY) / 4f;
                }
            }
        }
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
}
