using EventSystems;
using MD.Diggable;
using MD.Diggable.Gem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using MD.Character;

public static class MapDataTypeExtensions
{
    public static bool IsGem(this DiggableType type)
    {
        switch(type)
        {
            case DiggableType.CommonGem:
            case DiggableType.UncommonGem:
            case DiggableType.RareGem:
                return true;
            default: return false;
        }
    }

    public static bool IsBomb(this DiggableType type)
    {
        switch(type)
        {
            case DiggableType.NormalBomb:
                return true;
            default: return false;
        }
    }
}
public class MapManager : NetworkBehaviour, IMapManager
{

    #region SERIALIZE FIELDS
    [SerializeField]
    private Transform gemContainer = null;

    [SerializeField]
    private int generateZoneSideLength = 4;

    [SerializeField]
    private int minAmountPerZone = 2;

    [SerializeField]
    private int maxAmountPerZone = 3;

    [SerializeField]
    private float generateDelay = 2f;

    [SerializeField]
    private GameObject commonGem = null;
    [SerializeField]
    private GameObject uncommonGem = null;
    [SerializeField]
    private GameObject rareGem = null;
    #endregion

    #region FIELDS
    private int commonDropWeight = 10;
    private int uncommonDropWeight = 5;
    private int rareDropWeight = 2;

    // private int uncommonGemValue = 4;
    // private int commonGemValue = 1;
    // private int rareGemValue = 10;

    private Vector2Int mapSize = new Vector2Int(24,20);
    private int rootX = -12, rootY = -12;
    private float halfTileSize = .5f;
    private int[,] mapData;
    private GameObject[,] Diggables;

    // private int ToMapIndex(Vector2Int idx)
    // {   
    //     if (idx.x >= mapSize.x || idx.x < 0 || 
    //         idx.y >= mapSize.y || idx.y < 0)
    //     {
    //         return -1;
    //     }
    //     int index = idx.x*mapSize.y + idx.y;
    //     Debug.Log(mapData.Count);
    //     if (index < 0 || index >= mapData.Count)
    //     {
    //         // Debug.Log("failed at pos x:" +idx.x + " y:" + idx.y);
    //         return -1;
    //     }
    //     return index;
    // }

    private bool canGenerateNewGem;
    #endregion

    public ScanAreaData GetScanAreaData(Vector2[] posToScan) {
        // Debug.Log("se" + mapData.Length);
        return new ScanAreaData(GenTileData(posToScan).ToArray());    
    }
    
    private IEnumerable<ScanTileData> GenTileData(Vector2[] posToScan)
    {
        int res;
        foreach (var pos in posToScan)
        {
            try
            {
                res = mapData[(int)pos.x - rootX,(int) pos.y - rootY];
            }
            catch
            {
                res = 0;
            }
            yield return new ScanTileData(pos,res);
        }
    }

    // [Server]
    // public void RegisterMapManager()
    // {
    //     Debug.Log("Calling");
    //     RpcRegisterMapManager();
    // }

    // [ClientRpc]
    // void RpcRegisterMapManager()
    // {
    //     Debug.Log("registering mapmanager");
    //     ServiceLocator.Register<IMapManager>(GetComponent<IMapManager>());
    //     IMapManager imap;
    //     ServiceLocator.Resolve<IMapManager>(out imap);
    //     Debug.Log(imap);
    // }
    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        EventManager.Instance.StartListening<GemDigSuccessData>(HandleDigSuccess);
    }
    [Client]
    void Start()
    {
        // EventManager.Instance.StartListening<GemDigSuccessData>(RemoveGemFromMapData);
        mapData = new int[mapSize.x,mapSize.y];
    }
    // [Client]
    // void OnDestroy()
    // {
    //     EventManager.Instance.StopListening<GemDigSuccessData>(RemoveGemFromMapData);
    // }  
    [Client]
    public void NotifyNewGem(Vector2 pos, int diggable)
    {
        Vector2Int idx = PositionToIndex(pos);
        mapData[idx.x,idx.y] = diggable;
    }

    [Server]
    private void HandleDigSuccess(GemDigSuccessData gemDigSuccessData)
    {
        Vector2Int index = PositionToIndex(new Vector2(gemDigSuccessData.posX,gemDigSuccessData.posY));
        try
        {
            mapData[index.x,index.y] = 0;
            Diggables[index.x,index.y] = null;
            gemDigSuccessData.digger.GetComponent<MD.Character.Player>().IncreaseScore(gemDigSuccessData.value);
        }
        catch
        {
            Debug.Log("failed to remove gem at index " + index);
        }
    }

    [Server]
    public void GenerateMap()
    {
        mapData = new int[mapSize.x,mapSize.y];
        Diggables = new GameObject[mapSize.x,mapSize.y];
        GenerateGems();
        canGenerateNewGem = true;
        // StartCoroutine(GenerateNewGems());
    }

    [Server]
    private void GenerateGems()
    {
        int areaWidth = mapSize.x / generateZoneSideLength;
        int areaHeight = mapSize.y / generateZoneSideLength;
        int amtPerZone, nGeneratedGems;
        (GameObject prefab, int value) randomGem; 
        Vector2Int randomPos = Vector2Int.zero;
        for (int y = 0; y < generateZoneSideLength; y++)
        {
            for (int x = 0; x < generateZoneSideLength; x++)
            {
                amtPerZone = Random.Range(minAmountPerZone, maxAmountPerZone + 1);
                nGeneratedGems = 0;
                while (nGeneratedGems < amtPerZone)
                {
                    randomPos.x= Random.Range(0, areaWidth) + areaWidth* x;
                    randomPos.y = Random.Range(0, areaHeight) + areaHeight* y;
                    if (mapData[randomPos.x,randomPos.y] != 0) continue;

                    randomGem = GetRandomGem();
                    mapData[randomPos.x,randomPos.y] = randomGem.value;
                    GameObject Gem = Instantiate(randomGem.prefab, IndexToPosition(randomPos), 
                        Quaternion.identity, gemContainer);
                    NetworkServer.Spawn(Gem);
                    Diggables[randomPos.x,randomPos.y] = Gem;
                    nGeneratedGems++;
                }
            }
        }
        //generate gem-rich areas
    }
    
    [Server]
    private (GameObject, int) GetRandomGem()
    {
        int random = Random.Range(1, commonDropWeight + uncommonDropWeight + rareDropWeight + 1);
        if (random <= commonDropWeight)
        {
            return (commonGem, (int) DiggableType.CommonGem);
        }

        if (random <= commonDropWeight + uncommonDropWeight)
        {
            return (uncommonGem, (int) DiggableType.UncommonGem);
        }
        
        return (rareGem, (int) DiggableType.RareGem);
    }
    [Server]
    private Vector2Int GetRandomEmptyIndex()
    {
        Vector2Int randomPos = Vector2Int.zero;
        bool foundLocation  = false;
        
        int maxTries = 10;
        int timesTried = 0;

        while(!foundLocation)
        {
            randomPos.x = Random.Range(0, mapSize.x);
            randomPos.y = Random.Range(0, mapSize.y);
            if (mapData[randomPos.x,randomPos.y] == 0)
            {
                foundLocation = true;                
            }

            if (timesTried > maxTries)
            {
                Debug.Log("failed to get empty position's index");
                return -Vector2Int.one;
            }
            timesTried++;
        }
        return randomPos;
    }
    
    private Vector3 IndexToPosition(Vector2 index)
    {
        return new Vector3(index.x + rootX + halfTileSize, index.y + rootY + halfTileSize, 0f);
    }

    [Server]
    private IEnumerator GenerateNewGems()
    {
        WaitForSeconds waitTime = new WaitForSeconds(generateDelay);
        (GameObject prefab, int value) newGem;
        Vector2Int randomIndex; 
        Vector3 worldPostion;

        while(canGenerateNewGem)
        {
            yield return waitTime;
            randomIndex = GetRandomEmptyIndex();
            if (randomIndex == -Vector2Int.one)
            {
                continue;
            }
            newGem = GetRandomGem(); 
            worldPostion = IndexToPosition(randomIndex);
            mapData[randomIndex.x,randomIndex.y] = newGem.value;
            var gem =  Instantiate(newGem.prefab, worldPostion, Quaternion.identity, gemContainer);
            Diggables[randomIndex.x,randomIndex.y] = gem;
            NetworkServer.Spawn(gem);
            EventSystems.EventManager.Instance.TriggerEvent(
                new GemSpawnData(worldPostion.x - MapConstants.SPRITE_OFFSET.x, 
                worldPostion.y - MapConstants.SPRITE_OFFSET.y, (DiggableType)newGem.value));
        }
    }

    public Vector2Int GetMapSize() => mapSize;

    public Vector2Int PositionToIndex(Vector2 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x - rootX),Mathf.FloorToInt(position.y - rootY));
    }

    [Server]
    public bool TrySpawnDiggableAtIndex(Vector2Int idx, DiggableType diggable, GameObject prefab)
    {
        // if ( mapData[idx.x,idx.y] != (int) DiggableType.Empty)
        // { 
        //     return false;
        // }
        // mapData[idx.x, idx.y] = (int) diggable;
        // Instantiate(prefab, IndexToPosition(idx), Quaternion.identity, gemContainer);
        return true;
    }   



    [Server]
    public void DigAtPosition(NetworkIdentity player)
    {
        DigAction digger = player.GetComponent<DigAction>();
        Vector2Int index = PositionToIndex(player.transform.position);
        GameObject gem = null;
        try
        {
            gem = Diggables[index.x,index.y];
        }
        catch{return;}
        if (gem != null)
        {
            gem.GetComponent<GemObtain>().Dig(digger);
        }
    }

}
