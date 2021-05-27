using UnityEngine;
using Mirror;
using MD.Character.Animation;
using MD.Diggable.Core;
using System.Collections;
namespace MD.Character
{
    public class DigAction : NetworkBehaviour
    {
        [SerializeField]
        protected int power = 1;

        public int Power => power;

        protected virtual bool IsPlayer => true;


        public override void OnStartAuthority()
        {
            StartListeningToEvents();
        }

        private void OnDestroy()
        {
            StopListeningToEvents();
        }

        protected virtual void StartListeningToEvents()
        {
            EventSystems.EventManager.Instance.StartListening<DigAnimEndData>(HandleDigAnimEnd);
        }

        protected virtual void StopListeningToEvents()
        {
            EventSystems.EventManager.Instance.StopListening<DigAnimEndData>(HandleDigAnimEnd);
        }

        protected void HandleDigAnimEnd(DigAnimEndData _) => CmdDig(power);

        [Command]
        protected virtual void CmdDig(int digPower)
        {
            ServiceLocator
                .Resolve<IDiggableGenerator>()
                .Match(
                    unavailServiceErr => Debug.LogError(unavailServiceErr.Message),
                    diggableGenerator => 
                        diggableGenerator.DigAt(
                            connectionToClient.identity, 
                            Mathf.FloorToInt(transform.position.x), 
                            Mathf.FloorToInt(transform.position.y), 
                            digPower)                
                );
        }

        // // Cheat control
        // [ClientCallback]
        // private void Update()
        // {
        //     if (!hasAuthority)
        //     {
        //         return;
        //     }
            
        //     if (Input.GetKeyDown(KeyCode.E))
        //     {
        //         EventSystems.EventManager.Instance.TriggerEvent(new DigInvokeData());            
        //     }
        // }

        public void IncreaseDigPower(int amount, float time)
        {
            StartCoroutine(IncreaseDidPowerCoroutine(amount,time));
        }
        private IEnumerator IncreaseDidPowerCoroutine(int amount, float time)
        {
            power+= amount;
            yield return new WaitForSeconds(time);
            power-= amount;

            if (power <=0)
            {
                Debug.LogError("something went wrong");
            }
        }
    }
}