using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD.Character;
public class BotAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private float lastX, lastY;
    public void RevertToIdleState()
    {
        animator.SetBool(AnimatorConstants.IS_HOLDING, false);
    }

    public void SetHoldState()
    {
        animator.SetBool(AnimatorConstants.IS_HOLDING, true);
    }

    public void InvokeDig()
    {
        animator.SetTrigger(AnimatorConstants.INVOKE_DIG);
    }

    public void SetMovementState(Vector2 move)
    {
        var speed = move.sqrMagnitude;
        animator.SetFloat(AnimatorConstants.HORIZONTAL, move.x);
        animator.SetFloat(AnimatorConstants.VERTICAL, move.y);
        animator.SetFloat(AnimatorConstants.SPEED, speed);

        if (speed.IsEqual(0f))
        {
            PlayIdle();
            return;
        }

        BindLastMoveStats(move.x, move.y);
    }

    public void PlayIdle()
    {
        animator.SetFloat(AnimatorConstants.LAST_X, lastX);
        animator.SetFloat(AnimatorConstants.LAST_Y, lastY);
    }

    public void BindLastMoveStats(float lastX, float lastY)
    {
        this.lastX = lastX;
        this.lastY = lastY;
    }   
}
