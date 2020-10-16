using UnityEngine;

namespace MD.UI.MainMenu
{
    public class MainMenuTabToggler : MonoBehaviour
    {
        [SerializeField]
        private MenuTab[] tabs = null;
        
        public void Toggle(MenuType type)
        {            
            for (int i = 0; i < tabs.Length; i++)
            {
                if (tabs[i].MenuType.Equals(type)) tabs[i].Activate();
                else tabs[i].Deactivate();
            }
        }
    }
}