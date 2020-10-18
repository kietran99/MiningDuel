using MD.UI;
using UnityEngine;

namespace MD.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveAction : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1f;

        private Rigidbody2D rigidBody;
        private Vector2 moveVect, minMoveBound, maxMoveBound;
        private Vector2 offset = new Vector2(.5f, .6f);

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
#if UNITY_EDITOR
            var moveX = Input.GetAxisRaw("Horizontal");
            var moveY = Input.GetAxisRaw("Vertical");
            // Comment this line to move the player with joystick
            moveVect = new Vector2(moveX, moveY);
#endif
            MoveCharacter();
        }

        private void BindMoveVector(JoystickDragData data) => moveVect = data.InputDirection;

        private void MoveCharacter()
        {
            var movePos = rigidBody.position + moveVect * speed * Time.fixedDeltaTime;
            movePos = new Vector2(Mathf.Clamp(movePos.x, minMoveBound.x + offset.x, maxMoveBound.x - offset.x),
                                Mathf.Clamp(movePos.y, minMoveBound.y + offset.y, maxMoveBound.y - offset.y));
            rigidBody.MovePosition(movePos);

            
            EventSystems.EventManager.Instance.TriggerEvent(new MoveData(rigidBody.position.x, rigidBody.position.y));
        } 
        
        public void SetBounds(Vector2 minMoveBound, Vector2 maxMoveBound)
        {
            this.minMoveBound = minMoveBound;
            this.maxMoveBound = maxMoveBound;
        }
    }
}
