using MD.UI;
using UnityEngine;

namespace MD.Tutorial
{
    public class BasicTutorialMaterial : TutorialMaterial<MD.UI.JoystickDragData> 
    {
        protected override void HandleEvent(JoystickDragData dragData)
        {
            if (!dragData.InputDirection.Equals(UnityEngine.Vector2.zero))
            {
                base.HandleEvent(dragData);
            }
        }
    }
}