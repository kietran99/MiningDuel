using UnityEngine;

namespace MD.Character.Animation
{
    public class DigAnimationEvent : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        // override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
           
        // }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {                        
            animator.SetBool(MD.Character.AnimatorConstants.IS_DIGGING, false);
            bool isBot = animator.GetComponent<BotTag>();
            if (isBot)
            {
                //Debug.Log("BOT DIG");
                EventSystems.EventManager.Instance.TriggerEvent(new BotDigAnimEndData());
            }
            else 
            {
                //WARNING cheating here
                if (animator.transform.parent.GetComponent<Player>().hasAuthority)             
                //
                    EventSystems.EventManager.Instance.TriggerEvent(new DigAnimEndData());
            }
        }
       
        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
