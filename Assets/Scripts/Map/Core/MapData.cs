using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MapData 
{
    public int width;
    public int height;
    public int[] map;
    public MapData (ManualMapGenerator mapGen)
    {
        width = mapGen.MapWidth;
        height = mapGen.MapHeight;
        map = new int[width*height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                map[x*width + y] = mapGen.GetElement(x,y);
            }
        }
    }
    public int GetElement (int x, int y)
    {
        return map[x*width + y];
    }
}
