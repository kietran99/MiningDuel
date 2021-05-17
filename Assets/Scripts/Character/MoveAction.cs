﻿using MD.UI;
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
        private float speed = 1f;

        [SerializeField]
        private float knockbackForce = 2f;

        [SerializeField]
        private float counterSuccessDashDistance = 2f;

        private Rigidbody2D rigidBody;
        private Vector2 moveVect, minMoveBound, maxMoveBound;
        private Vector2 offset = new Vector2(.5f, .5f);
        
        [SerializeField] [SyncVar]
        private float speedModifier = 1f;

        [SyncVar]
        private int slowedDownCount = 0;
        private float SlowDownPercentage = .8f;

        private Player player = null;
        private Player Player
        {
            get
            {
                if (player != null) return player;
                return player = GetComponent<Player>();
            }
        }

        private bool isImmobilize = false;

        private void Start()
        {
            speedModifier = 1f;
            rigidBody = GetComponent<Rigidbody2D>();
            var eventConsumer = EventSystems.EventConsumer.GetOrAttach(gameObject);
            eventConsumer.StartListening<JoystickDragData>(BindMoveVector);
            eventConsumer.StartListening<DamageTakenData>(OnDamageTaken);
            eventConsumer.StartListening<GetCounteredData>(HandleGetCountered);
            eventConsumer.StartListening<CounterSuccessData>(OnCounterSuccessful);
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
            isImmobilize = true;
            Invoke(nameof(RegainMobility), counterData.immobilizeTime);
        }

        private void RegainMobility() => isImmobilize = false;

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

            if (moveVect.Equals(Vector2.zero) || !Player.CanMove) return;

            MoveCharacter(moveVect.x, moveVect.y);
        }

        private void BindMoveVector(JoystickDragData data) => moveVect = data.InputDirection;
        
        private void MoveCharacter(float moveX, float moveY)
        {
            var movePos = new Vector2(moveX, moveY).normalized * speed*speedModifier;
            if (slowedDownCount > 0) movePos*=(1f-SlowDownPercentage);
            // transform.Translate(movePos * Time.fixedDeltaTime);
            // transform.position = new Vector2(Mathf.Clamp(transform.position.x, minMoveBound.x + offset.x, maxMoveBound.x - offset.x),
            //                     Mathf.Clamp(transform.position.y, minMoveBound.y + offset.y, maxMoveBound.y - offset.y));
           rigidBody.MovePosition(rigidBody.position + movePos*Time.fixedDeltaTime);
        //    rigidBody.position = new Vector2(Mathf.Clamp(rigidBody.position.x, minMoveBound.x + offset.x, maxMoveBound.x - offset.x),
        //                         Mathf.Clamp(rigidBody.position.y, minMoveBound.y + offset.y, maxMoveBound.y - offset.y));
        } 

        private void LateUpdate()
        {
            if (!isLocalPlayer) return;

            if (moveVect.Equals(Vector2.zero) || !Player.CanMove) return;

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
            StartCoroutine(IncreaseSpeedCoroutine(percentage,time));
        }

        public void SlowDown(float time)
        {
            StartCoroutine(SlowDownCoroutine(time));
        }

        private IEnumerator SlowDownCoroutine(float time)
        {
            slowedDownCount += 1;
            yield return new WaitForSeconds(time);
            slowedDownCount -= 1;
        }

        private IEnumerator IncreaseSpeedCoroutine(float percentage, float time)
        {
            Debug.Log("increase speed by " + percentage);
            speedModifier += percentage;
            yield return new WaitForSeconds(time);
            speedModifier -= percentage;
        }
    }
}
