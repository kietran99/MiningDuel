using UnityEngine;

public class ScanTileData
{
    public ScanTileData(Vector2 position, int diggable)
    {
        Position = position;
        Diggable = diggable;
    }

    public Vector2 Position { get; set; }
    public int Diggable { get; set; }
}
