using MD.VisualEffects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName = string.Empty;

    [SerializeField]
    private FadeScreen fadeScreen = null;   
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fadeScreen.StartFading(() => SceneManager.LoadScene(nextSceneName));
        }
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
