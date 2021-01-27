﻿using EventSystems;
using MD.Diggable;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using MD.Character;
using UnityEngine.SceneManagement;

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

    public static bool IsProjectile(this DiggableType type)
    {
        switch(type)
        {
            case DiggableType.NormalBomb:
                return true;
            default: return false;
        }
    }

    public static DiggableType ToDiggable(this int value)
    {
        switch(value)
        {
            case (int) DiggableType.CommonGem:
                return DiggableType.CommonGem;
            case (int) DiggableType.UncommonGem:
                return DiggableType.UncommonGem;
            case (int) DiggableType.RareGem:
                return DiggableType.RareGem;
            case (int) DiggableType.NormalBomb:
                return DiggableType.NormalBomb;
            default: return DiggableType.Empty;
        }
    }
}

public class MapManager : NetworkBehaviour, IMapManager
{
    #region SERIALIZE FIELDS
    [SerializeField]
    private Transform gemContainer = null;

    [SerializeField]
    private int genZoneSideLength = 4;

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

    private Vector2Int mapSize = new Vector2Int(24,20);
    private int rootX = -12, rootY = -12;
    private float halfTileSize = .5f;
    private DiggableType[,] mapData;
    private GameObject[,] diggables;
    private PlayerItemSpawner itemSpawner = null;

    private bool canGenerateNewGem;
    #endregion

    public ScanAreaData GetScanAreaData(Vector2[] posToScan) => new ScanAreaData(GenTileData(posToScan).ToArray());    
    
    private IEnumerable<ScanTileData> GenTileData(Vector2[] posToScan)
    {
        foreach (var pos in posToScan)
        {
            yield return new ScanTileData(pos, TryGetDiggableAt(PositionToIndex(pos)));
        }
    }

    private int TryGetDiggableAt(Vector2Int idx)
    {        
        try 
        {
            return (int)mapData[(int)idx.x,(int)idx.y];
        }
        catch
        {
            return 0;
        }
    }
    
    public bool IsProjectileAt(Vector2 pos)
    {
        return TryGetDiggableAt(PositionToIndex(pos)).ToDiggable().IsProjectile();
    }

    public bool IsGemAt(Vector2 pos)
    {
        return TryGetDiggableAt(PositionToIndex(pos)).ToDiggable().IsGem();
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
        itemSpawner = GetComponent<PlayerItemSpawner>();
        EventManager.Instance.StartListening<ServerDiggableDestroyData>(HandleDigSuccess);
    }

    public override void OnStartClient()
    {
        mapData = new DiggableType[mapSize.x, mapSize.y];
        EventManager.Instance.StartListening<DiggableDestroyData>(RemoveDiggableFromMapData);
        EventManager.Instance.StartListening<DiggableSpawnData>(AddDiggableToMapData);
    }

    public override void OnStopClient()
    {
        EventManager.Instance.StopListening<DiggableDestroyData>(RemoveDiggableFromMapData);
        EventManager.Instance.StopListening<DiggableSpawnData>(AddDiggableToMapData);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        EventManager.Instance.StopListening<ServerDiggableDestroyData>(HandleDigSuccess);
    }

    [Client]
    private void AddDiggableToMapData(DiggableSpawnData data)
    {
        Vector2Int idx = PositionToIndex(new Vector2(data.posX,data.posY));
        try
        {
            mapData[idx.x, idx.y] = data.diggable.ToDiggable();
        }
        catch
        {
            Debug.Log("cant add  "+ data.diggable.ToDiggable() + " in mapdata at index " + idx);
        }        
    }

    [Server]
    private void HandleDigSuccess(ServerDiggableDestroyData diggableDestroyData)
    {
        //Debug.Log("handle dig success get called");
        Vector2Int index = PositionToIndex(new Vector2(diggableDestroyData.posX, diggableDestroyData.posY));     
        mapData[index.x, index.y] = 0;
        diggables[index.x, index.y] = null;                    

        if (diggableDestroyData.diggable.ToDiggable().IsGem())
        {
            PlayerBot bot;
            // var player = diggableDestroyData.digger.GetComponent<MD.Character.Player>();
            // if (player != null)
            // {
            //     player.IncreaseScore(diggableDestroyData.diggable);
            // }
            /*else */if (bot = diggableDestroyData.digger.GetComponent<PlayerBot>())
            {
                bot.score += diggableDestroyData.diggable;
            }

            return;
        }

        if (diggableDestroyData.diggable.ToDiggable().IsProjectile())
        {
            itemSpawner.SpawnBombAtPlayer(diggableDestroyData.digger.netIdentity);
        }
    }

    [ClientCallback]
    private void RemoveDiggableFromMapData(DiggableDestroyData data)
    {
        Vector2Int idx = PositionToIndex(new Vector2(data.posX, data.posY));
        
        try
        {
            mapData[idx.x, idx.y] = 0;
        }
        catch
        {
            Debug.Log("Can't remove " + data.diggable.ToDiggable() + " in mapdata at index " + idx);
        }

    }

    // [Server]
    // private void HandleDigSuccess(ProjectileObtainData data)
    // {
    //     Vector2Int index = PositionToIndex(new Vector2(data.posX,data.posY));
    //     try
    //     {
    //         mapData[index.x,index.y] = 0;
    //         Diggables[index.x,index.y] = null;
    //         //do something here
    //     }
    //     catch
    //     {
    //         Debug.Log("failed to remove projectile at index " + index);
    //     }       
    // }

    [Server]
    public void GenerateMap()
    {
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        mapData = new DiggableType[mapSize.x,mapSize.y];
        diggables = new GameObject[mapSize.x,mapSize.y];
        GenerateGems();
        canGenerateNewGem = true;
        //generate projectile if has this component
        ProjectileGenerator projGen = null;
        projGen = GetComponent<ProjectileGenerator>();
        if (projGen)
        {
            projGen.StartGenerate(GetComponent<IMapManager>());
        }
        // StartCoroutine(GenerateNewGems());
    }

    [Server]
    private void GenerateGems()
    {
        int areaWidth = mapSize.x / genZoneSideLength;
        int areaHeight = mapSize.y / genZoneSideLength;
        int amtPerZone, nGeneratedGems;
        (GameObject prefab, DiggableType value) randomGem; 
        Vector2Int randomPos = Vector2Int.zero;
        for (int y = 0; y < genZoneSideLength; y++)
        {
            for (int x = 0; x < genZoneSideLength; x++)
            {
                amtPerZone = Random.Range(minAmountPerZone, maxAmountPerZone + 1);
                nGeneratedGems = 0;
                while (nGeneratedGems < amtPerZone)
                {
                    randomPos.x= Random.Range(0, areaWidth) + areaWidth* x;
                    randomPos.y = Random.Range(0, areaHeight) + areaHeight* y;
                    if (mapData[randomPos.x,randomPos.y] != 0) continue;

                    randomGem = GetRandomGem();

                    GameObject instance = Instantiate(randomGem.prefab, IndexToPosition(randomPos), Quaternion.identity, gemContainer);
                    SpawnDiggable(instance,randomGem.value, randomPos.x, randomPos.y);
                    nGeneratedGems++;
                }
            }
        }
        //generate gem-rich areas
    }
    
    [Server]
    private (GameObject, DiggableType) GetRandomGem()
    {
        int random = Random.Range(1, commonDropWeight + uncommonDropWeight + rareDropWeight + 1);
        if (random <= commonDropWeight)
        {
            return (commonGem, DiggableType.CommonGem);
        }

        if (random <= commonDropWeight + uncommonDropWeight)
        {
            return (uncommonGem, DiggableType.UncommonGem);
        }
        
        return (rareGem, DiggableType.RareGem);
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
        (GameObject prefab, DiggableType value) newGem;
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
            var gem =  Instantiate(newGem.prefab, worldPostion, Quaternion.identity, gemContainer);
            SpawnDiggable(gem,newGem.value, randomIndex.x, randomIndex.y);
        }
    }

    public Vector2Int GetMapSize() => mapSize;

    private Vector2Int PositionToIndex(Vector2 position) => 
    new Vector2Int(Mathf.FloorToInt(position.x - (float)rootX), Mathf.FloorToInt(position.y - rootY));

    [Server]
    public bool TrySpawnAt(Vector2Int idx, DiggableType diggableType, GameObject diggable)
    {
        if (mapData[idx.x, idx.y] != DiggableType.Empty)
        { 
            return false;
        }

        var diggableInstance = Instantiate(diggable, IndexToPosition(idx), Quaternion.identity, gemContainer);
        SpawnDiggable(diggableInstance, diggableType, idx.x, idx.y);
        return true;
    }

    [Server]
    private void SpawnDiggable(GameObject diggable, DiggableType diggableType, int x, int y)
    {
        NetworkServer.Spawn(diggable);
        mapData[x,y] = diggableType;
        diggables[x,y] = diggable;
    }

    [Server]
    public void DigAt(NetworkIdentity player, Vector2 position)
    {
        DigAction digger = player.GetComponent<DigAction>();
        Vector2Int index = PositionToIndex(position);
        GameObject obj = null;

        try
        {
            obj = diggables[index.x, index.y];
        }
        catch
        {
            return;
        }

        if (obj == null) return;
        
        var diggable = obj.GetComponent<IDiggable>();

        if (diggable != null)
        {
            diggable.Dig(digger);
        }
        else
        {
            Debug.Log("Not a diggable");
        }       
    }
}
