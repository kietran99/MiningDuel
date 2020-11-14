using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InGameExit : MonoBehaviour
{    
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ExitToLobby);
    }

    private void ExitToLobby()
    {
        MD.Character.Player player;
        if (ServiceLocator.Resolve<MD.Character.Player>(out player)) player.ExistGame();
    }
}
