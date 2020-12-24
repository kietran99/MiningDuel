using UnityEngine;

public class TileNode : WeightedNode<Vector2Int>
{
    private const int MAX_SUCCESSORS = 8;

    private bool hasDiggable;

    private bool HasDiggable
    {
        get => hasDiggable;
        set
        {
            hasDiggable = value;

            if (value)
            {
                Weight = 0;
                return;
            }

            int nOccupiedNeighbors = 0;
            foreach (TileNode neighbor in Neighbors)
            {
                if (neighbor.hasDiggable) nOccupiedNeighbors++;
            }

            Weight = MAX_SUCCESSORS - nOccupiedNeighbors;
        }
    }

    public TileNode(Vector2Int value, int weight = MAX_SUCCESSORS, bool hasDiggable = false) : base(value, weight)
    {
        this.hasDiggable = hasDiggable;
    }

    public void OnDiggableSpawn()
    {
        HasDiggable = true;
        Neighbors.ForEach(successor => ((TileNode) successor).OnNeighborDiggableSpawn());
    }

    public void OnDiggableDug()
    {
        HasDiggable = false;
        Neighbors.ForEach(successor => ((TileNode) successor).OnNeighborDiggableDug());
    }

    public void OnNeighborDiggableSpawn()
    {
        Weight = Weight == 0 ? 0 : Weight - 1;
    }

    public void OnNeighborDiggableDug()
    {
        Weight = hasDiggable ? 0 : Weight + 1;
    }

    public override bool Equals(object obj)
    {
        return obj is TileNode && Value.Equals(((TileNode) obj).Value);
    }

    public override int GetHashCode()
    {
        return (10 * Value.x + 1 * Value.y).GetHashCode();
    }

    public void Log()
        {
            string neighborPositions = string.Empty;
            foreach (var pos in Neighbors)
            {
                neighborPositions += pos.Value + ", ";
            }
            string res = "Position = " + Value + " Weight = " + Weight;
            res = neighborPositions.Equals(string.Empty) ? res : res + " Neighbors = " + neighborPositions;
            Debug.Log(res);          
        }
}