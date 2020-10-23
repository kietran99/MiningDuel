using UnityEngine;
using UnityEngine.UI;

namespace MD.UI.MainMenu
{
    public class MenuTab : MonoBehaviour
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private MainMenuTabToggler toggler = null;

        [SerializeField]
        private MenuType type = MenuType.LOBBY;

        [SerializeField]
        private GameObject layout = null;
        
        [SerializeField]
        private Button activeButton = null, inactiveButton = null;
        #endregion

        public MenuType MenuType { get => type; }

        void Start()
        {
            inactiveButton.onClick.AddListener(Toggle);
        }
        
        private void Toggle() =>toggler.Toggle(type);

        public void Activate()
        {
            activeButton.gameObject.SetActive(true);
            inactiveButton.gameObject.SetActive(false);
            layout.SetActive(true);
        }

        public void Deactivate()
        {
            activeButton.gameObject.SetActive(false);
            inactiveButton.gameObject.SetActive(true);
            layout.SetActive(false);
        }
    }
}
