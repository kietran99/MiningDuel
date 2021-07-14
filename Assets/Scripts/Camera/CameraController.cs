using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private bool smoothFollow = true;

    [SerializeField]
    private float smoothSpeed = .125f;

    private Tilemap map = null;
    private Transform player;
    private Vector3 botLeftLimit, topRightLimit;

    private Vector3 offset;
    private Vector3 smoothVelocity = Vector3.zero;

    public void SetMapData(Tilemap tilemap) 
    {
        map = tilemap;
        MapConstants.MapMinBound = map.localBounds.min;
        MapConstants.MapMaxBound = map.localBounds.max;
        // Debug.Log("Local Bounds min:" + MapConstants.MapMinBound); 
        // Debug.Log("Local Bounds max:" + MapConstants.MapMaxBound); 
        Init(map.localBounds.min, map.localBounds.max);
    }

    public void Init(Vector3 botLeft, Vector3 topRight)
    {
        MapConstants.MapMinBound = map.localBounds.min;
        MapConstants.MapMaxBound = map.localBounds.max;
        
        if (!ServiceLocator.Resolve(out MD.Character.Player player)) return; 

        this.player = player.transform;
        var mainCamera = Camera.main;
        var camHalfHeight = mainCamera.orthographicSize;
        var camHalfWidth = mainCamera.aspect * camHalfHeight;
        
        offset = new Vector3(0f, 0f, transform.position.z);
        botLeftLimit = botLeft + new Vector3(camHalfWidth, camHalfHeight, 0f);
        topRightLimit = topRight - new Vector3(camHalfWidth, camHalfHeight, 0f);

        player.GetComponent<MD.Character.MoveAction>().SetBounds(map.localBounds.min, map.localBounds.max);
    }

    void LateUpdate()
    {
        if (player == null) return;

        var movedPos = smoothFollow 
                            ? Vector3.SmoothDamp(transform.position, player.position + offset, ref smoothVelocity, smoothSpeed)
                            : player.position + offset;

        transform.position = movedPos;
        
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, botLeftLimit.x, topRightLimit.x),
                                        Mathf.Clamp(transform.position.y, botLeftLimit.y, topRightLimit.y),
                                        transform.position.z);
    }
}
