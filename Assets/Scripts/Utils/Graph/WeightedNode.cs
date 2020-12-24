using System.Collections.Generic;

public class WeightedNode<T>
{
    public T Value { get; set; }
    public virtual int Weight { get; set; }
    public HashSet<WeightedNode<T>> Neighbors { get; set; }

    public WeightedNode(T value, int weight)
    {
        Value = value;
        Weight = weight;
        Neighbors = new HashSet<WeightedNode<T>>();
    }

    public void AddSuccessor(WeightedNode<T> successor)
    {
        if (successor.Equals(this)) return;

        Neighbors.Add(successor);
    }
}
