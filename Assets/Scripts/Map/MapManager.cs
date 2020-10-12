using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapManager :  MonoBehaviour, IMapManager
{
    // map's size = (width, Height) 
    private Vector2 mapSize = new Vector2(24,20);
    //array to store info of all diggable items in the map
    private int[,] mapData;

    //time wait between generating new gems
    private float generateDelay = 2f;
    private bool canGenerateNewGem;

    //return all scanTileData of a desired area
    public ScanAreaData GetScanAreaData(Vector2[] tilesToScan)
    {
        ScanAreaData result;
        ScanTileData[] tileData = new ScanTileData[tilesToScan.Length];
        ScanTileData tile;
        int idx = 0;
        foreach (Vector2 tilePos in tilesToScan)
        {
            //get tile corresponding to the tile pos 
            tile = new ScanTileData(tilePos, mapData[(int) tilePos.x,(int) tilePos.y]);
            tileData[idx] = tile;
            idx++;
        }
        result = new ScanAreaData(tileData);
        return result;
    }

    // generate map at the start of the match
    public GameObject gem;
    void Start()
    {
        GenerateMap();
    }
    public void GenerateMap()
    {
        mapData = new int[(int) mapSize.x,(int) mapSize.y];
        GenerateGems();
        // StartCoroutine(GenerateNewGems());
    }
    // generate base gem of the map
    private void GenerateGems()
    {
        //bottom left position of the map on grid
        int rootX = -12;
        int rootY = -12;
        float halfTileSize = .5f;
        //generate gems spreading equally on the map
        //devide map into n*n areas with equal number of gems
        //map size needs to be a multiple of n
        int n = 4;
        int areaWidth = (int) mapSize.x/n;
        int areaHeight = (int) mapSize.y/n;
        Debug.Log(areaHeight + areaWidth);
        //generate up to 2 gems in every area
        int gemPerArea;
        int numberOfGeneratedGems;
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x< n; x++)
            {
                gemPerArea = Random.Range(1,3);
                numberOfGeneratedGems = 0;
                while (numberOfGeneratedGems < gemPerArea)
                {
                    int randomX = Random.Range(0, areaWidth) + areaWidth*x;
                    int randomY = Random.Range(0, areaHeight) + areaHeight*y;
                    //if random postion havent has gem yet
                    if (mapData[randomX,randomY] == 0)
                    {
                        mapData[randomX,randomY] = 1;
                        Instantiate(gem,new Vector3(randomX + rootX + halfTileSize,randomY + rootY + halfTileSize,0),Quaternion.identity);
                        numberOfGeneratedGems++;
                    }
                }
            }
        }
        //generate gem-rich areas

    }
    // generate new gem every x second
    private IEnumerator GenerateNewGems()
    {
        while(canGenerateNewGem)
        {
            
            yield return new WaitForSeconds(generateDelay);
        }
    }

}
