using UnityEngine;
using UnityEngine.SceneManagement;

namespace MD.UI.MainMenu
{
    public class RoomJoin : MonoBehaviour
    {
        [SerializeField]
        private string sceneToLoad = string.Empty;
        
        public void Join()
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}