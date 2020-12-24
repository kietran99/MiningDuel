using UnityEngine;
using Functional.Type;
using System.Collections.Generic;

namespace MD.Map.Core
{
    public class MapTester : MonoBehaviour
    {
        private List<WeightedNode<char>> lst;

        void Start()
        {
            // diggableData.GetAccessAt(-2, 2).Match(
            //     err => Debug.Log("Invalid"),
            //     access => diggableData.Spawn(access, DiggableType.UncommonGem)
            // );
            // diggableData.Log();

            // diggableData.GetAccessAt(0, 0).Match(
            //     err => Debug.Log("Invalid"),
            //     access => diggableData.Reduce(access, 1)
            // );
            // diggableData.Log();
            
            // diggableData.GetAccessAt(5, 6).Match(
            //     err => Debug.Log("Invalid"),
            //     access => diggableData.Reduce(access, 4)
            // );
            // diggableData.Log();

            // diggableData.GetAccessAt(5, 6).Match(
            //     err => Debug.Log("Invalid"),
            //     access => diggableData.Reduce(access, 6)
            // );
            // diggableData.Log();            
            //lst = GenWeightedNodeList();
            //lst.LogExpectedRates();
            
            var positions = GenRandomTilePos();
            TileGraph tileGraph = new TileGraph(positions);
            Dictionary<Vector2Int, int> counter = new Dictionary<Vector2Int, int>();
            foreach (var pos in positions) counter.Add(pos, 0);                   
            tileGraph.OnDiggableSpawn(new Vector2Int(0, 0));
            tileGraph.OnDiggableSpawn(new Vector2Int(0, 2));
            tileGraph.OnDiggableDug(new Vector2Int(0, 2));
            //tileGraph.Log();
            tileGraph.LogExpectedRates();
            TestRandomTiles(tileGraph, counter);
        }   

        private void TestRandomTiles(TileGraph tileGraph, Dictionary<Vector2Int, int> counter)
        {
            Debug.Log("--------------------TEST RANDOM--------------------");
            for (int i = 0; i < 100; i++)
            {
                var rand = tileGraph.RandomTile();
                counter[rand] = counter[rand] + 1;
            }

            foreach (var i in counter)
            {
                Debug.Log(i.Key + ": " + i.Value);
            }
            Debug.Log("--------------------------------------------------");
        }

        private Vector2Int[] GenRandomTilePos()
        {
            return new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, 1),
                // new Vector2Int(1, 1),
                // new Vector2Int(1, 0),
                // new Vector2Int(-1, 1),
                // new Vector2Int(-1, 0),
                // new Vector2Int(-1, -1),
                // new Vector2Int(0, -1),
                // new Vector2Int(1, -1),
                new Vector2Int(0, 2),
                new Vector2Int(2, 3),
                new Vector2Int(-1, 4),
                new Vector2Int(2, 4)
            };
        }

        void Update() 
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                TestGen();
            }
        }

        private void TestGen()
        {
            int a = 0, b = 0, c = 0, d = 0;
            for (int i = 0; i < 100; i++)
            {
                var res = lst.RandomUnsortedList();

                if (res == 'a') 
                {
                    a++;
                    continue;
                }

                if (res == 'b')
                {
                    b++;
                    continue;
                }

                if (res == 'c')
                {
                    c++;
                    continue;
                }

                d++;
            }

            Debug.Log("a: " + a);
            Debug.Log("b: " + b);
            Debug.Log("c: " + c);
            Debug.Log("d: " + d);
        }

        private List<WeightedNode<char>> GenWeightedNodeList()
        {
            return new List<WeightedNode<char>>()
            {
                new WeightedNode<char>('a', 4),
                new WeightedNode<char>('b', 6),
                new WeightedNode<char>('c', 3),
                new WeightedNode<char>('d', 1),
            };
        }  
    }
}