using System.Collections.Generic;
using UnityEngine;

public class TileGraph
{
    private class NodeComparer : Comparer<TileNode>
    {
        public override int Compare(TileNode firstNode, TileNode secondNode)
        {
            return firstNode.Weight.CompareTo(secondNode.Weight);
        }
    }

    private List<TileNode> tileNodes;
    private TileNode comparingObj = new TileNode(new Vector2Int(0, 0));

    public TileGraph(Vector2Int[] tilePositions)
    {
        tileNodes = new List<TileNode>(MakeTileNodes(tilePositions)/*, new NodeComparer()*/);
        //Debug.Log(tileNodes.Count);
        foreach (var node in tileNodes)
        {
            ConnectSuccessors(node);
        }
    }

    private IEnumerable<TileNode> MakeTileNodes(Vector2Int[] tilePositions)
    {
        for (int i = 0, size = tilePositions.Length; i < size; i++)
        {
            yield return new TileNode(tilePositions[i]);
        }
    }

    private void ConnectSuccessors(TileNode node)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0) continue;

                var neighborPos = new Vector2Int(node.Value.x + x, node.Value.y + y);
                if (!TryGetTile(neighborPos, out TileNode neighborNode)) continue;
                
                if (neighborNode.Neighbors.Contains(node)) continue;

                node.AddSuccessor(neighborNode);
                neighborNode.AddSuccessor(node);
            }
        }
    }

    private bool ContainsTile(Vector2Int pos)
    {
        comparingObj.Value = pos;
        return tileNodes.Contains(comparingObj);
    }

    private bool TryGetTile(Vector2Int pos, out TileNode result)
    {
        if (!ContainsTile(pos))
        {
            result = null;
            return false;
        }

        foreach (var tile in tileNodes)
        {
            if (tile.Value.Equals(pos))
            {
                result = tile;
                return true;
            }
        }

        result = null;
        return false;
    }

    public Vector2Int RandomTile()
    {
        return RandomUnsortedList();
    }

    private Vector2Int RandomUnsortedList()
    {
        SortH2LByWeight();

        float randVal = UnityEngine.Random.value * tileNodes.GetWeightSum();
            
        for (int i = 0, size = tileNodes.Count; i < size; i++)
        {
            if ((randVal -= tileNodes[i].Weight) < 0)
            {
                return tileNodes[i].Value;
            }
        }

        return tileNodes[tileNodes.Count - 1].Value;
    }

    private void SortH2LByWeight()
    {
        for (int i = 0, size = tileNodes.Count; i < size; i++)
        {
            for (int j = i; j > 0; j--)
            {
                if (tileNodes[j].Weight < tileNodes[j - 1].Weight)
                {
                    break;
                }

                var temp = tileNodes[j];
                tileNodes[j] = tileNodes[j - 1];
                tileNodes[j - 1] = temp;
            }
        }
    }

    public void OnDiggableSpawn(Vector2Int pos)
    {
        if (!TryGetTile(pos, out TileNode spawnTile)) return;

        spawnTile.OnDiggableSpawn();
    }

    public void OnDiggableDug(Vector2Int pos)
    {
        if (!TryGetTile(pos, out TileNode spawnTile)) return;

        spawnTile.OnDiggableDug();
    }

    public void Log()
    {
        Debug.Log("--------------------TILE GRAPH--------------------");
        
        foreach (var entry in tileNodes)
        {
            entry.Log();
        }

        Debug.Log("--------------------------------------------------");
    }

    public void LogExpectedRates()
    {
        tileNodes.LogExpectedRates();
    }
}
