using UnityEngine;
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
        if (ServiceLocator.Resolve<MD.Character.Player>(out MD.Character.Player player)) player.ExitGame();
    }
}
