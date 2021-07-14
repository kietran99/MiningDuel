using UnityEngine;

namespace MD.UI
{
    public class AliveCountView : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text text = null;

        void Start()
        {
            UpdateView(new Character.AliveCountChangeData((Mirror.NetworkManager.singleton as NetworkManagerLobby).Players.Count));
            EventSystems.EventConsumer.Attach(gameObject).StartListening<Character.AliveCountChangeData>(UpdateView);
        }

        private void UpdateView(Character.AliveCountChangeData data)
        {
            text.text = data.nAlive.ToString();
        }
    }
}
