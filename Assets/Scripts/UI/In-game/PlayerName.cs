using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class PlayerName : MonoBehaviour
    {        
        [SerializeField]
        private MD.Character.Player player = null;

        [SerializeField]
        private Text playerNameText = null;

        [SerializeField]
        private Vector2 baseOffset = new Vector2(0f, 150f);

        private Camera mainCamera;

        private void Start()
        {
            if (player.isLocalPlayer)
            {
                gameObject.SetActive(false);
                return;
            }

            playerNameText.text = player.PlayerName;
            playerNameText.color = player.PlayerColor;
            gameObject.AddComponent<EventSystems.EventConsumer>().StartListening<EndGameData>(Hide);
        }

        private void Hide(EndGameData _) => playerNameText.enabled = false;

        private void FixedUpdate()
        {
            playerNameText.transform.position = GetFollowOffset(player.transform.position);
        }

        private Vector3 GetFollowOffset(Vector3 playerPos)
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            var screenPos = mainCamera.WorldToScreenPoint(playerPos);
            
            return new Vector3(screenPos.x + baseOffset.x, screenPos.y + baseOffset.y, transform.position.z);
        }
    }
}
