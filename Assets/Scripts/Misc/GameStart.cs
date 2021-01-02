using MD.VisualEffects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName = string.Empty;

    [SerializeField]
    private FadeScreen fadeScreenAnimator = null;

    private void Start()
    {
        EventSystems.EventManager.Instance.StartListening<FadeStartCompleteData>(HandleFadeStartComplete);
    }

    private void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<FadeStartCompleteData>(HandleFadeStartComplete);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fadeScreenAnimator.StartFading();
        }
    }

    private void HandleFadeStartComplete(FadeStartCompleteData data) => LoadMainMenu();

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
