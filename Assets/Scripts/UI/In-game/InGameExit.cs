using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InGameExit : MonoBehaviour
{
    [SerializeField]
    private GameObject endGameStatsContainer = null;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ExitToLobby);
    }

    private void ExitToLobby()
    {
        SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
    }
}
