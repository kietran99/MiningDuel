using UnityEngine;

namespace MD.UI
{
    public class JoystickDragData : EventSystems.IEventData
    {        
        public Vector2 InputDirection { get; set; }

        public JoystickDragData(Vector2 inputDirection)
        {
            InputDirection = inputDirection;
        }
    }
}