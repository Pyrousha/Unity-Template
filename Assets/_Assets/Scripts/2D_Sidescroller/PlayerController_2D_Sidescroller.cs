using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_2D_Sidescroller : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelSpeed_ground;
    [SerializeField] private float frictionSpeed_ground;
    [SerializeField] private float turnaroundAccelMultiplier_ground;
    [Space(5)]
    [SerializeField] private float accelSpeed_air;
    [SerializeField] private float frictionSpeed_air;
    [SerializeField] private float turnaroundAccelMultiplier_air;
    [Space(5)]
    [SerializeField] private float terminalVelocity;

    [Header("References")]
    [SerializeField] private SpriteRenderer sprRend;
    [SerializeField] private Transform interactParentToRotate;
    [SerializeField] private BoxCollider2D groundCheckBoxArea;

    [Header("Ground-Checking and Gravity")]
    [SerializeField] private LayerMask groundLayer;
    [Space(5)]
    [SerializeField] private float gravityStrength_weak = 1;
    [SerializeField] private float gravityStrength_strong = 1;
    [SerializeField] private float jumpStrength = 1;
    [SerializeField] private float coyoteTimeDuration = 0.0625f;
    [SerializeField] private float spaceReleaseJumpHeightMultiplier = 0.75f;
    private bool holdingJump = false;

    private Rigidbody2D rb;
    bool grounded = false;

    private float lastGroundedTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region determine if player is grounded or not
        bool prevIsGrounded = grounded;

        grounded = rb.linearVelocityY < 0.25f && (Physics2D.OverlapBox(groundCheckBoxArea.transform.position, groundCheckBoxArea.size * 0.9f, 0, groundLayer) != null);
        if (grounded)
            lastGroundedTime = Time.time;

        if(prevIsGrounded && !grounded)
        {
            //Just fell off a platform
            holdingJump = false;
        }
        #endregion

        #region Apply Gravity + Check for Jump
        if (Time.time - coyoteTimeDuration <= lastGroundedTime && InputHandler.Instance.Jump.Down)
        {
            rb.linearVelocityY = jumpStrength;
            holdingJump = true;
            grounded = false;
        }
        else if(!grounded)
        {
            //Check for jump release
            if(holdingJump && !InputHandler.Instance.Jump.Holding)
            {
                rb.linearVelocityY *= spaceReleaseJumpHeightMultiplier;
                holdingJump = false;
            }

            //Apply gravity
            if(holdingJump)
            {
                rb.linearVelocityY -= gravityStrength_weak;
                if (rb.linearVelocityY < 0)
                    holdingJump = false;
            }
            else
            {
                rb.linearVelocityY -= gravityStrength_strong;
            }

            if (rb.linearVelocityY < -terminalVelocity)
                rb.linearVelocityY = -terminalVelocity;
        }
        #endregion

        #region Acceleration
        //Get gravityless velocity
        float currXVelocity = rb.linearVelocityX;
        float targXVelocity = InputHandler.Instance.MoveXZ.x * maxSpeed;

        float currAcceleration;
        float currFriction;
        float currTurnaroundAccelMultiplier;
        if (grounded)
        {
            currAcceleration = accelSpeed_ground;
            currFriction = frictionSpeed_ground;
            currTurnaroundAccelMultiplier = turnaroundAccelMultiplier_ground;
        }
        else
        {
            currAcceleration = accelSpeed_air;
            currFriction = frictionSpeed_air;
            currTurnaroundAccelMultiplier = turnaroundAccelMultiplier_air;
        }

        if (targXVelocity < 0)
        {
            //Check if needing to accelerate to the left
            if (currXVelocity > targXVelocity)
            {
                //Check for turnaround acceleration multiplier
                if (currXVelocity > 0)
                    currAcceleration *= currTurnaroundAccelMultiplier;

                currXVelocity = Mathf.Max(targXVelocity, currXVelocity - currAcceleration);
            }
        }
        else if (targXVelocity > 0)
        {
            //Check if needing to accelerate to the right
            if (currXVelocity < targXVelocity)
            {
                //Check for turnaround acceleration multiplier
                if (currXVelocity < 0)
                    currAcceleration *= currTurnaroundAccelMultiplier;

                currXVelocity = Mathf.Min(targXVelocity, currXVelocity + currAcceleration);
            }
        }
        else
        {
            //Apply friction
            if (currXVelocity > 0)
                currXVelocity = Mathf.Max(0, currXVelocity - currFriction);
            else if (currXVelocity < 0)
                currXVelocity = Mathf.Min(0, currXVelocity + currFriction);
        }

        rb.linearVelocityX = currXVelocity;
        #endregion

        if (currXVelocity < 0)
        {
            sprRend.flipX = true;
            interactParentToRotate.localScale = new Vector3(-1, 0, 0);
        }
        else if (currXVelocity > 0)
        {
            sprRend.flipX = false;
            interactParentToRotate.localScale = new Vector3(1, 0, 0);
        }
    }
}
