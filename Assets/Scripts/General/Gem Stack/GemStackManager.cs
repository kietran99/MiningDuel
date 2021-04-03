using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Diggable.Gem;
using MD.CraftingSystem;

namespace MD.CraftingSystem
{

    public class GemStackManager : NetworkBehaviour
    {
        [SerializeField]
        private CraftingRecipe recipeSO;

        [SerializeField]
        private int StackSize = 15;

        private int stackSize = 0;

        [SerializeField]
        private DiggableType[] gemStack;
        [SerializeField]
        private int head; //start index of the stack
        [SerializeField]
        private int tail; //last index of the stack ; empty stack tail = -1;

        [SerializeField]
        List<CraftItemName> canCraftItemList;

        public override void OnStartAuthority()
        {
            base.OnStartClient();
            gemStack = new DiggableType[StackSize];
            head = 0;
            tail = 0;
            stackSize = 0;
            //just in case
            for (int i=0; i< gemStack.Length; i++) gemStack[i] = DiggableType.EMPTY;


            EventSystems.EventManager.Instance.StartListening<GemObtainData>(HandleGemObtain);

        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();
            EventSystems.EventManager.Instance.StopListening<GemObtainData>(HandleGemObtain);
        }

        private void AddToStack(DiggableType type)
        {
            //empty stack
            if (stackSize == 0)
            {
                gemStack[head] = type;            
                tail = head;
                stackSize++;
                return;
            }

            tail++;
            if (tail >= gemStack.Length) tail = 0;

            //full stack
            if (tail == head)
            {
                head++;
                if (head >= gemStack.Length) head = 0;
            }

            gemStack[tail] = type;
            stackSize++;
        }

        private void RemoveFromStack(int index, int length)
        {
            int fillIndex = index;
            int currentIndex = index + length;
            if (currentIndex >= gemStack.Length) currentIndex-= gemStack.Length;
            for (int i = index; i < index + length; i++)
            {
                int j = i;
                if (j >= gemStack.Length) j-= gemStack.Length;
                gemStack[j] = DiggableType.EMPTY;
            }
            while (currentIndex != (tail + 1))
            {
                gemStack[fillIndex] = gemStack[currentIndex];
                gemStack[currentIndex] = DiggableType.EMPTY;
                fillIndex++;
                if (fillIndex >= gemStack.Length) fillIndex -= gemStack.Length;
                currentIndex++;
                if (currentIndex >= gemStack.Length) currentIndex -= gemStack.Length;
            }
            tail = fillIndex -1;

            stackSize-=length;

            canCraftItemList.Clear();
            for (int i=0 ; i< gemStack.Length; i++)
            {
                List<CraftItemName> res = CanCraft(i);
                canCraftItemList.AddRange(res);
            }
        }

        private int GetIndex(int position)
        {
            int res = head + position;
            if (res >= gemStack.Length) res-= gemStack.Length;
            return res;
        }


        private void HandleGemObtain(GemObtainData data)
        {
            // Debug.Log("receive gem obtain data " + data.type);
            AddToStack(data.type);
            canCraftItemList.Clear();
            if (stackSize <3) return;
            for (int i = 0; i<= stackSize - 2 ; i++) // ignore 2 last gems
            {
                List<CraftItemName> res = CanCraft(GetIndex(i));
                canCraftItemList.AddRange(res);
            }
        }

        #if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                int length = 3;
                // if (Random.Range(0,2) == 1) length +=2;
                int max = tail;
                if (tail < head) max += gemStack.Length;
                max -= length;
                int randomIndex = Random.Range(head, max + 1);
                // Debug.Log("random from index " + head + " length " + (max + 1));
                if (randomIndex >= gemStack.Length) randomIndex -= gemStack.Length;
                // Debug.Log("remove at index " + randomIndex + " length " + length);
                RemoveFromStack(randomIndex,length);
            }
        }
        #endif


        private List<CraftItemName> CanCraft(int index)
        {
            List<CraftItemName> itemNames = new List<CraftItemName>();
            DiggableType[] materials = new DiggableType[recipeSO.MAX_NO_MATERIALS];
            //get materials
            for (int i = 0; i < recipeSO.MAX_NO_MATERIALS; i++)
            {
                int currentIndex = index  + i;
                if (currentIndex >= gemStack.Length) currentIndex -= gemStack.Length;
                materials[i] = gemStack[currentIndex]; 
                //if end of stack fill the rest with empty
                if (currentIndex == tail)
                {
                    for (int j = i+1; j< materials.Length; j++)
                    {
                        materials[j] = DiggableType.EMPTY;
                    }
                    break;
                }


            }

            foreach(Recipe recipe in recipeSO.Recipes)
            {
                for (int i = 0; i<= recipe.Materials.Length; i++)
                {
                    if (i == recipe.Materials.Length)
                    {
                        itemNames.Add(recipe.craftItemName);
                        break;
                    }
                    if (materials[i] == DiggableType.EMPTY || materials[i] != (DiggableType )recipe.Materials[i]) break;
                }
            }

            return itemNames;
        }

    }
}