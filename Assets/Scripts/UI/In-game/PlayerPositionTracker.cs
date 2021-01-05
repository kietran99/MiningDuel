using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PlayerPositionTracker : TargetTracker
{        
    private Camera mainCamera;
    private Vector2 baseOffset = new Vector2(0f, 130f);

    protected override void Start()
    {
        base.Start();
        GetComponent<Text>().text = playerTransform.GetComponent<MD.Character.Player>().PlayerName;
        mainCamera = Camera.main;
        Debug.Log(baseOffset);
    }

    protected override Vector3 GetFollowOffset(Vector3 playerPos)
    {
        var screenPos = mainCamera.WorldToScreenPoint(playerPos);
        
        return new Vector3(screenPos.x + baseOffset.x, screenPos.y + baseOffset.y, transform.position.z);
    }
}
