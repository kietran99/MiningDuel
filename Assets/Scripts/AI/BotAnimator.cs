using UnityEngine;
using MD.Character;

public class BotAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator animator = null;
    private float lastX, lastY;

    private void Start() 
    {
        var eventManager = EventSystems.EventManager.Instance;
        eventManager.StartListening<BotDigInvokeData>(HandleDigEvent);
    }

    private void OnDestroy() 
    {
        var eventManager = EventSystems.EventManager.Instance;
        eventManager.StopListening<BotDigInvokeData>(HandleDigEvent);
    }

    private void HandleDigEvent(BotDigInvokeData obj)
    {
        InvokeDig();
    }

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
        animator.SetBool(AnimatorConstants.IS_DIGGING, true);
    }

    public void SetMovementState(Vector2 move)
    {
        var speed = move.sqrMagnitude;
        if (speed < 0.1f) return;

        animator.SetFloat(AnimatorConstants.HORIZONTAL, move.x);
        animator.SetFloat(AnimatorConstants.VERTICAL, move.y);
        animator.SetFloat(AnimatorConstants.SPEED, speed);

        // if (speed.IsEqual(0f))
        // {
        //     PlayIdle();
        //     return;
        // }

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
