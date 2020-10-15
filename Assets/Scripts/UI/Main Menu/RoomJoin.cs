using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomJoin : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad = string.Empty;

    void Start()
    {
        
    }

    public void Join()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
