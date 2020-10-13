using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager :  MonoBehaviour, IMapManager
{
    #region SERIALIZE FIELDS
    [SerializeField]
    private Transform gemContainer = null;

    [SerializeField]
    private int generateZoneSideLength = 4;

    [SerializeField]
    private int maxAmountPerZone = 2;

    public GameObject gem;
    #endregion

    #region FIELDS
    private Vector2 mapSize = new Vector2(24,20);
    float rootX = -12f;
    float rootY = -12f;
    private float halfTileSize = .5f;
    private int[,] mapData;

    private float generateDelay = 2f;
    private bool canGenerateNewGem;
    #endregion

    public ScanAreaData GetScanAreaData(Vector2[] posToScan)
    {
        //ScanTileData[] tileData = new ScanTileData[posToScan.Length];

        //ScanTileData tile;
        //int idx = 0;
        //foreach (Vector2 tilePos in posToScan)
        //{
        //    tile = new ScanTileData(tilePos, mapData[(int) tilePos.x,(int) tilePos.y]);
        //    tileData[idx] = tile;
        //    idx++;
        //}
        
        //posToScan.Map((pos, idx) => tileData[idx] = new ScanTileData(pos, mapData[(int) pos.x, (int) pos.y]));
        //return new ScanAreaData(tileData);
        return new ScanAreaData(GenTileData(posToScan).ToArray());
    }
    
    private IEnumerable<ScanTileData> GenTileData(Vector2[] posToScan)
    {
        foreach (var pos in posToScan)
        {
            yield return new ScanTileData(pos, mapData[(int) pos.x - 12, (int) pos.y - 12]);
        }
    }

    void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        mapData = new int[(int) mapSize.x, (int) mapSize.y];
        GenerateGems();
        // StartCoroutine(GenerateNewGems());
    }
    private void GenerateGems()
    {
        //bottom left position of the map on grid
        int rootX = -12;
        int rootY = -12;
        float halfTileSize = .5f;

        int areaWidth = (int) mapSize.x/ generateZoneSideLength;
        int areaHeight = (int) mapSize.y/ generateZoneSideLength;
        Debug.Log(areaHeight + areaWidth);

        int amtPerZone, nGeneratedGems;

        for (int y = 0; y < generateZoneSideLength; y++)
        {
            for (int x = 0; x < generateZoneSideLength; x++)
            {
                amtPerZone = Random.Range(1, maxAmountPerZone + 1);
                nGeneratedGems = 0;

                while (nGeneratedGems < amtPerZone)
                {
                    int randomX = Random.Range(0, areaWidth) + areaWidth * x;
                    int randomY = Random.Range(0, areaHeight) + areaHeight * y;
                    
                    if (mapData[randomX, randomY] != 0) continue;

                    mapData[randomX, randomY] = 1;
                    Instantiate(gem, new Vector3(randomX + rootX + halfTileSize, randomY + rootY + halfTileSize, 0f),
                        Quaternion.identity, gemContainer);
                    nGeneratedGems++;
                }
            }
        }
        //generate gem-rich areas

    }

    private Vector3 IndexToPosition(Vector2 index)
    {
        return new Vector3(index.x + rootX + halfTileSize, index.y + rootY + halfTileSize, 0f);
    }

    private IEnumerator GenerateNewGems()
    {
        while(canGenerateNewGem)
        {
            
            yield return new WaitForSeconds(generateDelay);
        }
    }

}
