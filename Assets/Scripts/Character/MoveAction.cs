using MD.UI;
using UnityEngine;
using Mirror;
using System.Collections;

namespace MD.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveAction : NetworkBehaviour
    {
        private float DASH_MULTIPLIER = 1000f;
        [SerializeField]
        MD.VisualEffects.SlowEffect slowEffect = null;

        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private float knockbackForce = 2f;

        [SerializeField]
        private float counterSuccessDashDistance = 2f;

        private Rigidbody2D rigidBody;
        private Vector2 moveVect, minMoveBound, maxMoveBound;
        
        [SyncVar]
        private float speedModifier = 1f;

        [SyncVar]
        private int slowedDownCount = 0;
        private float SlowDownPercentage = .7f;
        private Player player = null;
        private bool isImmobilize = false;

        private void Start()
        {
            player = GetComponent<Player>();
            speedModifier = 1f;
            rigidBody = GetComponent<Rigidbody2D>();
            var eventConsumer = EventSystems.EventConsumer.GetOrAttach(gameObject);
            eventConsumer.StartListening<JoystickDragData>(BindMoveVector);
            eventConsumer.StartListening<DamageTakenData>(OnDamageTaken);
            eventConsumer.StartListening<GetCounteredData>(HandleGetCountered);
            eventConsumer.StartListening<CounterSuccessData>(OnCounterSuccessful);           
        }

        public override void OnStartServer()
        {
            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<ExplodedData>(ServerHandleExploded);
        }

        private void OnDamageTaken(DamageTakenData data)
        {
            if (!data.damagedId.Equals(netId))
            {
                return;
            }

            Dash(data.atkDir * knockbackForce);
        }

        private void HandleGetCountered(GetCounteredData counterData)
        {
            Immobilize(counterData.immobilizeTime);
        }

        [Server]
        private void ServerHandleExploded(ExplodedData data)
        {
            RpcHandleExploded(data.explodedTargetID, data.immobilizeTime);
        }

        [ClientRpc]
        private void RpcHandleExploded(uint explodedPlayerId, float immobilizeTime)
        {
            if (netId != explodedPlayerId)
            {
                return;
            }

            Immobilize(immobilizeTime);
        }

        private void Immobilize(float time)
        {
            if (hasAuthority)
            {
                EventSystems.EventManager.Instance.TriggerEvent(new StunStatusData(true));
            }

            isImmobilize = true;
            Invoke(nameof(RegainMobility), time);
        }

        private void RegainMobility() 
        {
            if (hasAuthority)
            {
                EventSystems.EventManager.Instance.TriggerEvent(new StunStatusData(false));
            }

            isImmobilize = false;
        }

        private void OnCounterSuccessful(CounterSuccessData data)
        {
            Dash(data.counterDir * counterSuccessDashDistance);
        }

        private void Dash(Vector2 vect)
        {
            rigidBody.AddForce(vect * DASH_MULTIPLIER, ForceMode2D.Impulse);
        }

        void FixedUpdate()
        {
            if (!isLocalPlayer) return;

        #if UNITY_EDITOR
            var moveX = Input.GetAxisRaw("Horizontal");
            var moveY = Input.GetAxisRaw("Vertical");
            EventSystems.EventManager.Instance.TriggerEvent(new JoystickDragData(new Vector2(moveX, moveY)));
        #endif
        
            if (isImmobilize)
            {
                return;
            }

            if (moveVect.Equals(Vector2.zero) || !player.CanMove) return;

            MoveCharacter(moveVect.x, moveVect.y);
        }

        private void BindMoveVector(JoystickDragData data) => moveVect = data.InputDirection;
        
        private void MoveCharacter(float moveX, float moveY)
        {
            var movePos = new Vector2(moveX, moveY).normalized * speed*speedModifier;
            if (slowedDownCount > 0) movePos*=(1f-SlowDownPercentage);
           rigidBody.MovePosition(rigidBody.position + movePos*Time.fixedDeltaTime);
        } 

        private void LateUpdate()
        {
            if (!isLocalPlayer) return;

            if (moveVect.Equals(Vector2.zero) || !player.CanMove) return;

            EventSystems.EventManager.Instance.TriggerEvent(new MoveData(rigidBody.position.x, rigidBody.position.y));
        }
        
        public void SetBounds(Vector2 minMoveBound, Vector2 maxMoveBound)
        {
            this.minMoveBound = minMoveBound;
            this.maxMoveBound = maxMoveBound;
        }

        [Command]
        public void CmdModifySpeed(float percentage, float time)
        {
            RpcRaiseSpeedBoostEvent(netId, time);
            StartCoroutine(IncreaseSpeedCoroutine(percentage,time));
        }

        [ClientRpc]
        private void RpcRaiseSpeedBoostEvent(uint playerId, float time)
        {
            EventSystems.EventManager.Instance.TriggerEvent(new SpeedBoostData(netId, time));
        }

        [Server]
        public void SlowDown(float time)
        {
            StartCoroutine(SlowDownCoroutine(time));
        }

        [Server]
        private IEnumerator SlowDownCoroutine(float time)
        {
            RpcPlaySlowEffect();
            slowedDownCount += 1;
            yield return new WaitForSeconds(time);
            slowedDownCount -= 1;
            if (slowedDownCount<=0) RpcStopSlowEffect();
        }

        [ClientRpc]
        private void RpcPlaySlowEffect()
        {
            slowEffect.Play();
        }

        [ClientRpc]
        private void RpcStopSlowEffect()
        {
            slowEffect.Stop();
        }

        private IEnumerator IncreaseSpeedCoroutine(float percentage, float time)
        {
            Debug.Log("Increase speed by " + percentage);
            speedModifier += percentage;
            yield return new WaitForSeconds(time);
            speedModifier -= percentage;
        }
    }
}
