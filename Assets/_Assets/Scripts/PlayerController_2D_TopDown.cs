using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_2D_TopDown : Singleton<PlayerController_2D_TopDown>
{
    [Header("Parameters")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelSpeed_ground;
    [SerializeField] private float frictionSpeed_ground;

    [Header("Ground-Checking and Gravity (No need to touch if not using gravity)")]
    [SerializeField] private bool checkForGrounded = false;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform raycastParent;
    [SerializeField] private float raycastHeight = 0.05f;
    [Space(5)]
    [SerializeField] private float gravityStrength = -1f;
    [Space(5)]
    [SerializeField] private float accelSpeed_air;
    [SerializeField] private float frictionSpeed_air;


    private Rigidbody rb;
    private List<Transform> raycastPoints = new List<Transform>();
    bool grounded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (checkForGrounded)
            for (int i = 0; i < raycastParent.childCount; i++)
            {
                raycastPoints.Add(raycastParent.GetChild(i));
            }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region determine if player is grounded or not
        grounded = false; //number of raycasts that hit the ground 

        if (checkForGrounded)
            foreach (Transform point in raycastPoints)
            {
                if (Physics.Raycast(point.position, -transform.up, out RaycastHit hit, raycastHeight, groundLayer))
                {
                    grounded = true;
                    break;
                }
            }
        else
            grounded = true;
        #endregion

        #region Apply Gravity
        if (gravityStrength > 0)
            rb.velocity -= new Vector3(0, gravityStrength, 0);
        #endregion


        #region Acceleration
        //Get gravityless velocity
        Vector3 noGravVelocity = rb.velocity;
        noGravVelocity.z = 0;

        //XZ Friction + acceleration
        Vector3 currInput;
        if (DialogueUI.Instance.isOpen)
            currInput = Vector3.zero;
        else
        {
            currInput = new Vector3(InputHandler.Instance.MoveXZ.x, InputHandler.Instance.MoveXZ.y, 0);
            if (currInput.magnitude > 1f)
                currInput.Normalize();
        }

        float accelSpeedToUse;
        float frictionSpeedToUse;
        if (grounded)
        {
            accelSpeedToUse = accelSpeed_ground;
            frictionSpeedToUse = frictionSpeed_ground;
        }
        else
        {
            accelSpeedToUse = accelSpeed_air;
            frictionSpeedToUse = frictionSpeed_air;
        }

        if (grounded)
        {
            //Apply ground fricion
            Vector3 velocity_local_friction = noGravVelocity.normalized * Mathf.Max(0, noGravVelocity.magnitude - frictionSpeedToUse);

            Vector3 updatedVelocity = velocity_local_friction;

            if (currInput.magnitude > 0.05f) //Pressing something, try to accelerate
            {
                Vector3 velocity_friction_and_input = velocity_local_friction + currInput * accelSpeedToUse;

                if (velocity_local_friction.magnitude <= maxSpeed)
                {
                    //under max speed, accelerate towards max speed
                    updatedVelocity = velocity_friction_and_input.normalized * Mathf.Min(velocity_friction_and_input.magnitude, maxSpeed);
                }
                else //Over max speed
                {
                    if (velocity_friction_and_input.magnitude <= maxSpeed) //Input below max speed
                        updatedVelocity = velocity_friction_and_input;
                    else
                    {
                        //Can't use input directly, since would be over max speed

                        //Would accelerate more, so don't user player input
                        if (velocity_friction_and_input.magnitude > velocity_local_friction.magnitude)
                            updatedVelocity = velocity_local_friction;
                        else
                            //Would accelerate less, user player input (since input moves velocity more to 0,0 than just friciton)
                            updatedVelocity = velocity_friction_and_input;
                    }
                }
            }

            //Apply velocity
            updatedVelocity.z = rb.velocity.z;
            rb.velocity = updatedVelocity;
        }
        #endregion
    }
}
