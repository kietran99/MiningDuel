using System.Collections.Generic;
using UnityEngine;

public static class WeightedNodeExtension
{
    public static T RandomSortedList<T>(this List<WeightedNode<T>> lst)
    {
        float randVal = UnityEngine.Random.value * lst.GetWeightSum();
            
        for (int i = 0, size = lst.Count; i < size; i++)
        {
            if ((randVal -= lst[i].Weight) < 0)
            {
                return lst[i].Value;
            }
        }

        return lst[lst.Count - 1].Value;
    }

    public static T RandomSortedSet<T>(this IEnumerable<WeightedNode<T>> set)
    {
        float randVal = UnityEngine.Random.value * set.GetWeightSum();
            
        foreach (var node in set)
        {
            if ((randVal -= node.Weight) < 0)
            {
                return node.Value;
            }
        }

        //return set[set.Count - 1].Value;
        return default(T);
    }

    public static int GetWeightSum<T>(this List<WeightedNode<T>> lst)
    {
        int sum = 0;

        for (int i = 0, size = lst.Count; i < size; i++)
        {
            sum += lst[i].Weight;
        }

        return sum;
    }

    public static int GetWeightSum<T>(this IEnumerable<WeightedNode<T>> iter)
    {
        int sum = 0;

        foreach (var node in iter)
        {
            sum += node.Weight;
        }

        return sum;
    }

    public static void SortH2LByWeight<T>(this List<WeightedNode<T>> lst)
    {
        for (int i = 0, size = lst.Count; i < size; i++)
        {
            for (int j = i; j > 0; j--)
            {
                if (lst[j].Weight < lst[j - 1].Weight)
                {
                    break;
                }

                var temp = lst[j];
                lst[j] = lst[j - 1];
                lst[j - 1] = temp;
            }
        }
    }

    // public static void LogExpectedRates<T>(this List<WeightedNode<T>> lst)
    // {
    //     Debug.Log("--------------------EXPTECTED RATES--------------------");

    //     int sum = lst.GetWeightSum();
    //     foreach (var node in lst)
    //     {
    //         Debug.Log(node.Value + ": " + node.Weight * 100 / sum);
    //     }

    //     Debug.Log("-------------------------------------------------------");
    // }

    public static void LogExpectedRates<T>(this IEnumerable<WeightedNode<T>> iter)
    {
        Debug.Log("--------------------EXPTECTED RATES--------------------");

        int sum = iter.GetWeightSum();
        foreach (var node in iter)
        {
            Debug.Log(node.Value + ": " + node.Weight * 100 / sum);
        }

        Debug.Log("-------------------------------------------------------");
    }
}
