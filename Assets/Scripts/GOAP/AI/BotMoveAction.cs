using UnityEngine;
using MD.Character;
using MD.Diggable.Projectile;
using Mirror;

public class BotMoveAction : NetworkBehaviour
{
    public float speed = 3f;
    private RaycastHit2D[] rayArr = new RaycastHit2D[10];
    // private int resCount = 0;
    // private bool collideLeft = false;
    // private float collideLeftDistance = 0f;
    // private bool collideRigth = false;

    // private float collideRightDistance = 0f;
    // private bool collideAhead = false;
    private Vector2 movePos = Vector2.zero;

    private bool isMoving = false;

    private BotAnimator animator;

    private void MoveBot()
    {
        if (isMoving) 
        {
            // resCount =  Physics2D.RaycastNonAlloc(transform.position, transform.forward, rayArr);
            
            
            if (Vector2.Distance(movePos,transform.position) < .1f)
            {
                transform.position = movePos;
                isMoving = false;
                return;
            }

            Vector2 moveDir = movePos - (Vector2)transform.position;
            animator.SetMovementState(moveDir);
            transform.Translate(moveDir.normalized*speed*Time.fixedDeltaTime);
        }
    }

    void FixedUpdate()
    {
        if (!hasAuthority) return;
        MoveBot();
    }

    public void SetAnimator(BotAnimator anim) => animator = anim;
    public bool IsMoving() => isMoving;
    public void startMoving() => isMoving = true;
    public void SetMovePos(Vector2 movePos) => this.movePos = movePos;
}
