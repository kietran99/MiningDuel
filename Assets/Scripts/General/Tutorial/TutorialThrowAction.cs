using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialThrowAction : MonoBehaviour
    {
        [SerializeField]
        private float basePower = 100f;

        private Camera mainCamera = null;
        private TutorialProjectileLauncher launcher;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {    
            if (launcher == null)
            {
                return;
            }

            if (!Input.GetMouseButtonDown(0))
            {
                return; 
            }

            // Vector2 clickPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPos = new Vector2(2f, 0f);
            Vector2 throwDir = clickPos - new Vector2(transform.position.x, transform.position.y);
            Throw(basePower, throwDir.x, throwDir.y);
            EventSystems.EventManager.Instance.TriggerEvent(new UI.ThrowInvokeData());
        }

        public void SetLauncher(TutorialProjectileLauncher launcher) 
        {
            this.launcher = launcher;
        }

        private void Throw(float power, float dirX, float dirY) => launcher.Launch(power, dirX, dirY);      
    }
}
