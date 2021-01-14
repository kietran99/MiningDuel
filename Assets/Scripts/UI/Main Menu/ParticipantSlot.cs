using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class ParticipantSlot : MonoBehaviour
    {
        [SerializeField]
        private WaitingForPlayer playerNameText = null;

        [SerializeField]
        private GameObject playerStateContainer = null;

        [SerializeField]
        private Image panel = null;

        public void ToggleEmptyState()
        {
            playerNameText.ShowWaitingForPlayer();
            playerStateContainer.SetActive(false);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, .5f);
        }

        public void ToggleOccupiedState(string playerName, bool isReady)
        {
            playerNameText.ShowPlayerName(playerName);
            playerStateContainer.SetActive(isReady);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 1f);
        }       
    }
}
