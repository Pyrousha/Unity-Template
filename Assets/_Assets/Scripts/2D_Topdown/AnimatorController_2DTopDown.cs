using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController_2DTopDown : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;

    [SerializeField] private float idleCutoffSpeed = 0.1f;

    private enum MoveStateEnum
    {
        UpIdle,
        DownIdle,
        LeftIdle,
        RightIdle,
        Up,
        Down,
        Left,
        Right
    }
    private MoveStateEnum state;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > idleCutoffSpeed)
        {
            //Moving
            if (Mathf.Abs(rb.linearVelocity.x * 2) >= Mathf.Abs(rb.linearVelocity.y))
            {
                //play horizontal move animation
                if (rb.linearVelocity.x > 0)
                    ChangeAnimationState(MoveStateEnum.Right);
                else
                    ChangeAnimationState(MoveStateEnum.Left);
            }
            else
            {
                //play vertical move animation
                if (rb.linearVelocity.y > 0)
                    ChangeAnimationState(MoveStateEnum.Up);
                else
                    ChangeAnimationState(MoveStateEnum.Down);
            }
        }
        else
        {
            //Not moving
            switch (state)
            {
                case MoveStateEnum.Up:
                    ChangeAnimationState(MoveStateEnum.UpIdle);
                    break;
                case MoveStateEnum.Down:
                    ChangeAnimationState(MoveStateEnum.DownIdle);
                    break;
                case MoveStateEnum.Left:
                    ChangeAnimationState(MoveStateEnum.LeftIdle);
                    break;
                case MoveStateEnum.Right:
                    ChangeAnimationState(MoveStateEnum.RightIdle);
                    break;
            }
        }
    }

    private void ChangeAnimationState(MoveStateEnum _newState)
    {
        if (state == _newState)
            return;

        state = _newState;
        anim.Play(_newState.ToString(), 0);
    }
}
