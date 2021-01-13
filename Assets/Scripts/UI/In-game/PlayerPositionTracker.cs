using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
[RequireComponent(typeof(Text))]
    public class PlayerPositionTracker : TargetTracker
    {        
        private Camera mainCamera;
        private Text playerNameText;
        private Vector2 baseOffset = new Vector2(0f, 130f);

        protected override void Start()
        {
            base.Start();
            playerNameText = GetComponent<Text>();
            playerNameText.enabled = true;
            playerNameText.text = playerTransform.GetComponent<MD.Character.Player>().PlayerName;
            mainCamera = Camera.main;
            EventSystems.EventManager.Instance.StartListening<EndGameData>(Hide);
        }

        void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<EndGameData>(Hide);
        }

        private void Hide(EndGameData endGameData)
        {
            playerNameText.enabled = false;
        }

        protected override Vector3 GetFollowOffset(Vector3 playerPos)
        {
            var screenPos = mainCamera.WorldToScreenPoint(playerPos);
            
            return new Vector3(screenPos.x + baseOffset.x, screenPos.y + baseOffset.y, transform.position.z);
        }
    }
}
