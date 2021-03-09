using MD.UI;
using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialMoveAction : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private UnityEngine.Tilemaps.Tilemap map = null;

        private Rigidbody2D rigidBody;
        private Vector2 moveVect, minMoveBound, maxMoveBound;
        private Vector2 offset = new Vector2(.5f, .5f);

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            minMoveBound = map.localBounds.min + new Vector3(.6f, .2f, 0f);
            maxMoveBound = map.localBounds.max - new Vector3(.6f, .6f, 0f);
            GetComponent<EventSystems.EventConsumer>().StartListening<JoystickDragData>(BindMoveVector);
        }

        void FixedUpdate()
        {
#if UNITY_EDITOR
            var moveX = Input.GetAxisRaw("Horizontal");
            var moveY = Input.GetAxisRaw("Vertical");
            EventSystems.EventManager.Instance.TriggerEvent(new JoystickDragData(new Vector2(moveX, moveY)));
#endif
            // if (moveVect.Equals(Vector2.zero) || !Player.CanMove) return;
            
            MoveCharacter(moveVect.x, moveVect.y);
        }

        private void BindMoveVector(JoystickDragData data) => moveVect = data.InputDirection;
        
        private void MoveCharacter(float moveX, float moveY)
        {
            var movePos = new Vector2(moveX, moveY).normalized * speed;
            transform.Translate(movePos * Time.fixedDeltaTime);
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, minMoveBound.x + offset.x, maxMoveBound.x - offset.x),
                                Mathf.Clamp(transform.position.y, minMoveBound.y + offset.y, maxMoveBound.y - offset.y));
        } 

        private void LateUpdate()
        {
            // if (moveVect.Equals(Vector2.zero) || !Player.CanMove) return;

            EventSystems.EventManager.Instance.TriggerEvent(new MoveData(rigidBody.position.x, rigidBody.position.y));
        }
    }
}
