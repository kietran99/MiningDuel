using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Tilemap map = null;

    private Transform player;
    private Vector3 botLeftLimit, topRightLimit;
    
    void Awake()
    {
        MapConstants.MapMinBound = map.localBounds.min;
        MapConstants.MapMaxBound = map.localBounds.max;
    }

    void Start()
    {
        Init(map.localBounds.min, map.localBounds.max);
    }

    private void Init(Vector3 botLeft, Vector3 topRight)
    {
        if (!ServiceLocator.Resolve(out MD.Character.Player player)) return; 

        this.player = player.transform;
        var mainCamera = Camera.main;
        var camHalfHeight = mainCamera.orthographicSize;
        var camHalfWidth = mainCamera.aspect * camHalfHeight;
        
        botLeftLimit = botLeft + new Vector3(camHalfWidth, camHalfHeight, 0f);
        topRightLimit = topRight - new Vector3(camHalfWidth, camHalfHeight, 0f);
        player.GetComponent<MD.Character.MoveAction>().SetBounds(botLeft, topRight);
    }

    void LateUpdate()
    {
        if (player == null) return;

        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, botLeftLimit.x, topRightLimit.x),
                                        Mathf.Clamp(transform.position.y, botLeftLimit.y, topRightLimit.y),
                                        transform.position.z);
    }
}
