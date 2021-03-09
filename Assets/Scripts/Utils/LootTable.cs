using System.Collections.Generic;
using UnityEngine;

public class LootEntry<Tag>
{
    public Tag tag;
    public int weight = 0;
}

public class LootTable<Entry, Tag> where Entry : LootEntry<Tag>
{
    [SerializeField]
    private Entry[] entries = null;

    private List<WeightedNode<Tag>> nodeBasedLootTable;

    private List<WeightedNode<Tag>> NodeBasedLootTable 
    {
        get 
        {
            if (nodeBasedLootTable == null)
            {
                nodeBasedLootTable = Init(entries);
            }

            return nodeBasedLootTable;
        }
    }

    public Tag Random => NodeBasedLootTable.RandomSortedList();

    public void Log() => NodeBasedLootTable.LogExpectedRates();

    private List<WeightedNode<Tag>> Init(Entry[] entries)
    {
        var nodeBasedLootTable = new List<WeightedNode<Tag>>();
        var duplicateCheckSet = new HashSet<Tag>();            

        entries
            .ForEach(entry => 
                {
                    if (duplicateCheckSet.Contains(entry.tag))
                    {
                        Debug.LogWarning("LOOT TABLE: Duplicate entry " + entry.tag);
                        return;
                    }

                    nodeBasedLootTable.Add(new WeightedNode<Tag>(entry.tag, entry.weight));
                    duplicateCheckSet.Add(entry.tag);
                }
            );

        nodeBasedLootTable.SortH2LByWeight();
        return nodeBasedLootTable;
    }
}
