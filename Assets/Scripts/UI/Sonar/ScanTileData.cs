using UnityEngine;

public class ScanTileData
{
    public ScanTileData(Vector2 position, DiggableType diggable)
    {
        Position = position;
        Type = diggable;
    }

    public Vector2 Position { get; set; }
    public DiggableType Type { get; set; }
}
