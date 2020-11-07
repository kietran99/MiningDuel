using MD.UI;
using UnityEngine;
using Mirror;

namespace MD.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Player))]
    public class MoveAction : NetworkBehaviour
    {
        [SerializeField]
        private float speed = 1f;

        private Rigidbody2D rigidBody;
        private Vector2 moveVect, minMoveBound, maxMoveBound;
        private Vector2 offset = new Vector2(.5f, .5f);

        void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            EventSystems.EventManager.Instance.StartListening<JoystickDragData>(BindMoveVector);
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<JoystickDragData>(BindMoveVector);
        }

        void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            
#if UNITY_EDITOR
            var moveX = Input.GetAxisRaw("Horizontal");
            var moveY = Input.GetAxisRaw("Vertical");
            EventSystems.EventManager.Instance.TriggerEvent(new JoystickDragData(new Vector2(moveX, moveY)));
            // MoveCharacter(moveX, moveY);
#endif
            MoveCharacter(moveVect.x, moveVect.y);
        }

        private void BindMoveVector(JoystickDragData data) => moveVect = data.InputDirection;
        
        private void MoveCharacter(float moveX, float moveY)
        {
            var movePos = new Vector2(moveX, moveY).normalized * speed;
            transform.Translate(movePos * Time.fixedDeltaTime);
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, minMoveBound.x + offset.x, maxMoveBound.x - offset.x),
                                Mathf.Clamp(transform.position.y, minMoveBound.y + offset.y, maxMoveBound.y - offset.y));
            // rigidBody.MovePosition(movePos*Time.fixedDeltaTime);
            EventSystems.EventManager.Instance.TriggerEvent(new MoveData(rigidBody.position.x, rigidBody.position.y));
        } 
        
        public void SetBounds(Vector2 minMoveBound, Vector2 maxMoveBound)
        {
            this.minMoveBound = minMoveBound;
            this.maxMoveBound = maxMoveBound;
        }
    }
}
