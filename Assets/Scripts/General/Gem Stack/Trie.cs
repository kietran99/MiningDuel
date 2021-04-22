using System;
using System.Collections.Generic;
using UnityEngine;
namespace MD.CraftingSystem
{
    [System.Serializable]
    public class Trie 
    {
        [System.Serializable]
        class TrieNode
        {
            public static int maxNOChildren = 4;
            public int[] children;
            public CraftItemName name;
            public TrieNode()
            {
                children = new int[maxNOChildren];
                this.name = CraftItemName.None;
                for (int i = 0; i < maxNOChildren; i++)
                    children[i] = -1;
            }

        };
        [SerializeField]
        List<TrieNode> trieNodesList; //since unity cant serialize linked nodes

        [SerializeField]
        TrieNode root; 

        [SerializeField]
        List<CraftableGem> indicesDict;

        public Trie(int maxNOChildren)
        {
           TrieNode.maxNOChildren = maxNOChildren;
           indicesDict = new List<CraftableGem>();
           trieNodesList = new List<TrieNode>();
           root = new TrieNode();
        }

        private int GetIndex(CraftableGem name)
        {
            int value = indicesDict.IndexOf(name);
            if (value != -1)
            {
                Debug.Log("found index is " + value);
                return value;
            }
            else
            {
                indicesDict.Add(name);
                Debug.Log("found index is " + (indicesDict.Count -1));
                return indicesDict.Count -1;
            }
        }
        

        public void Insert(CraftableGem[] gems, CraftItemName name)
        {
            TrieNode pCrawl = root;
            int index;
            for(int i = 0; i < gems.Length; i++)
            {
                index = GetIndex(gems[i]);
                if (pCrawl.children[index] == -1)
                {
                    TrieNode trieNode = new TrieNode();
                    trieNodesList.Add(trieNode);
                    pCrawl.children[index] = trieNodesList.Count -1;
                }

                // pCrawl = pCrawl.children[index];
                pCrawl = trieNodesList[pCrawl.children[index]];
            }

            pCrawl.name = name;
        }
        
        public CraftItemName Search(CraftableGem[] gems)
        {
            Debug.Log("start search");
            foreach (CraftableGem gem in gems)
            {
                Debug.Log(" " + gem);
            }
            int index;
            TrieNode pCrawl = root;
        
            for (int  i = 0; i < gems.Length; i++)
            {
                index = GetIndex(gems[i]);
        
                if (pCrawl.children[index] == -1)
                    return CraftItemName.None;
        
                // pCrawl = pCrawl.children[index];
                pCrawl = trieNodesList[pCrawl.children[index]];
            }
        
            if (pCrawl == null) return CraftItemName.None;
            return pCrawl.name;
        }
        
    }
}