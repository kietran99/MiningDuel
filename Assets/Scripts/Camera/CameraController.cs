using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    // [SerializeField]
    // private Tilemap map = null;

    // private Transform player;
    // private Vector3 botLeftLimit, topRightLimit;

    // void Start()
    // {
    //     player = Player.Instance.transform;

    //     var mainCamera = Camera.main;
    //     var camHalfHeight = mainCamera.orthographicSize;
    //     var camHalfWidth = mainCamera.aspect * camHalfHeight;

    //     botLeftLimit = map.localBounds.min + new Vector3(camHalfWidth, camHalfHeight, 0f);
    //     topRightLimit = map.localBounds.max - new Vector3(camHalfWidth, camHalfHeight, 0f);

    //     player.GetComponent<MD.Character.MoveAction>().SetBounds(map.localBounds.min, map.localBounds.max);
    // }

    // void LateUpdate()
    // {
    //     transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);

    //     transform.position = new Vector3(Mathf.Clamp(transform.position.x, botLeftLimit.x, topRightLimit.x),
    //                                     Mathf.Clamp(transform.position.y, botLeftLimit.y, topRightLimit.y),
    //                                     transform.position.z
    //         );
    // }
}
