public class ScanAreaData
{
    public ScanTileData[] Tiles { get; set; }

    public ScanTileData this[int idx] => Tiles[idx];

    public ScanAreaData(ScanTileData[] tiles)
    {
        Tiles = tiles;
    }
}
