using Mirror;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : NetworkBehaviour
{
    [SerializeField]
    private Tilemap map = null;

    private Transform player;
    private Vector3 botLeftLimit, topRightLimit;

    private void Awake()
    {
        Player.OnPlayerSpawn += StartFollowing;
    }

    private void OnDestroy()
    {
        Player.OnPlayerSpawn -= StartFollowing;
    }

    private void StartFollowing(GameObject player)
    {
        this.player = player.transform;
        DisableIfNotLocal();
    }

    private void DisableIfNotLocal()
    {
        if (player.GetComponent<Player>().isLocalPlayer) { return; }

        GetComponent<Camera>().enabled = false;
    }

    void Start()
    {
        // player = Player.Instance.transform;
        //player = Player.LocalPlayer.transform;
        var mainCamera = Camera.main;
        var camHalfHeight = mainCamera.orthographicSize;
        var camHalfWidth = mainCamera.aspect * camHalfHeight;
        return;
        botLeftLimit = map.localBounds.min + new Vector3(camHalfWidth, camHalfHeight, 0f);
        topRightLimit = map.localBounds.max - new Vector3(camHalfWidth, camHalfHeight, 0f);

        // player.GetComponent<MD.Character.MoveAction>().SetBounds(map.localBounds.min, map.localBounds.max);
    }

    void LateUpdate()
    {        
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        return;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, botLeftLimit.x, topRightLimit.x),
                                        Mathf.Clamp(transform.position.y, botLeftLimit.y, topRightLimit.y),
                                        transform.position.z
            );
    }
}
