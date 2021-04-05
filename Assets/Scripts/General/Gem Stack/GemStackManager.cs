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
        [System.Serializable]
        public class CraftableItemsData
        {
            public  CraftItemName name;
            public  int index;
            public  int length;

            public CraftableItemsData(CraftItemName name, int index, int length)
            {
                this.name = name;
                this.index = index;
                this.length = length;
            }

            public bool Equals (CraftableItemsData other)
            {
                if (name.Equals(other.name) && index.Equals(other.index) && length.Equals(other.length)) return true;
                return false;
            }
        }

        [SerializeField]
        private CraftingRecipe recipeSO;

        [SerializeField]
        private int MAX_NO_SLOTS = 15;

        [SerializeField]
        private int stackSize = 0;

        [SerializeField]
        private DiggableType[] gemStack;
        [SerializeField]
        private int head; //start index of the stack
        [SerializeField]
        private int tail; //last index of the stack ;

        [SerializeField]
        List<CraftableItemsData> craftableItemsList;
        
        [SerializeField]
        int SelectedIndex = 0;

        public override void OnStartAuthority()
        {
            base.OnStartClient();
            gemStack = new DiggableType[MAX_NO_SLOTS];
            craftableItemsList = new List<CraftableItemsData>();
            head = 0;
            tail = 0;
            stackSize = 0;
            //just in case
            for (int i=0; i< gemStack.Length; i++) gemStack[i] = DiggableType.EMPTY;


            EventSystems.EventManager.Instance.StartListening<GemObtainData>(HandleGemObtain);
            EventSystems.EventManager.Instance.StartListening<CraftMenuChangeIndexData>(HandleChangeIndex);
            EventSystems.EventManager.Instance.StartListening<UseItemInvokeData>(HandleUseItemInvoke);

        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();
            EventSystems.EventManager.Instance.StopListening<GemObtainData>(HandleGemObtain);
            EventSystems.EventManager.Instance.StopListening<CraftMenuChangeIndexData>(HandleChangeIndex);
            EventSystems.EventManager.Instance.StopListening<UseItemInvokeData>(HandleUseItemInvoke);
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
            else
            {
                stackSize++;
            }

            gemStack[tail] = type;
        }

        private void RemoveFromStack(int index, int length)
        {

            //remove gems used;
            for (int i = index; i < index + length; i++)
            {
                int j = i;
                if (j >= gemStack.Length) j-= gemStack.Length;
                gemStack[j] = DiggableType.EMPTY;
            }
            int currentIndex = 0, fillIndex = -1;

            //update stack
            for (int i = 0; i< stackSize - (GetPos(index) + length) ; i++)    
            {
                currentIndex = i + index + length;
                fillIndex = i + index; 
                if (fillIndex >= gemStack.Length) fillIndex -= gemStack.Length;
                if (currentIndex >= gemStack.Length) currentIndex -= gemStack.Length;

                gemStack[fillIndex] = gemStack[currentIndex];
                gemStack[currentIndex] = DiggableType.EMPTY;

            }
            stackSize-=length;
            
            //no update stack done
            if (fillIndex == -1)
            {
                if (stackSize <=0) fillIndex =0;
                else
                {
                    fillIndex = index-1;
                    if (fillIndex < 0) fillIndex += gemStack.Length;
                }
            }  

            tail = fillIndex;

            craftableItemsList.Clear();
            for (int i=0 ; i< stackSize - 2; i++)
            {
                List<CraftableItemsData> res = CanCraft(GetIndex(i));
                craftableItemsList.AddRange(res);
            }
            EventSystems.EventManager.Instance.TriggerEvent<CraftableItemsListChangeData>(new CraftableItemsListChangeData(GetItemListData()));

            EventSystems.EventManager.Instance.TriggerEvent<GemStackUsedData>(new GemStackUsedData(GetPos(index),length));
        }

        private List<CraftItemName> GetItemListData()
        {
            List<CraftItemName> res = new List<CraftItemName>();
            foreach (CraftableItemsData data in craftableItemsList)
            {
                res.Add(data.name);
            }
            return res;
        }

        private int GetIndex(int position)
        {
            int res = head + position;
            if (res >= gemStack.Length) res-= gemStack.Length;
            return res;
        }

        private void HandleGemObtain(GemObtainData data)
        {
            if (!recipeSO.IsGemCraftable(data.type)) return;
            AddToStack(data.type);
            if (stackSize <3) return;
            List<CraftableItemsData> tempList = new List<CraftableItemsData>();
            for (int i = 0; i< stackSize - 2 ; i++) // ignore 2 last gems
            {
                List<CraftableItemsData> res = CanCraft(GetIndex(i));
                tempList.AddRange(res);
            }
            if (!IsEqual(craftableItemsList,tempList))
            {
                craftableItemsList = tempList;
                EventSystems.EventManager.Instance.TriggerEvent<CraftableItemsListChangeData>(new CraftableItemsListChangeData(GetItemListData()));
            }
        }

        private bool IsEqual(List<CraftableItemsData> list1, List<CraftableItemsData> list2)
        {
            if (list1.Count != list2.Count) return false;
            for (int i=0; i< list1.Count ; i++)
            {
                if (!list1[i].Equals(list2[i])) return false;
            }
            return true;
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

        private int GetPos(int index)
        {
            int pos = index - head;
            if (pos < 0) pos += gemStack.Length;
            Debug.Log("pos is " + pos + " index  is " + index);
            return pos;
        }


        private List<CraftableItemsData> CanCraft(int index)
        {
            List<CraftableItemsData> itemNames = new List<CraftableItemsData>();
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
                        itemNames.Add(new CraftableItemsData(recipe.craftItemName,index,recipe.Materials.Length));
                        break;
                    }
                    if (materials[i] == DiggableType.EMPTY || materials[i] != (DiggableType )recipe.Materials[i]) break;
                }
            }

            return itemNames;
        }

        private void HandleChangeIndex(CraftMenuChangeIndexData data) => SelectedIndex = data.index;
        private void HandleUseItemInvoke(UseItemInvokeData data)
        {
            if (craftableItemsList.Count < 1) return;
            if (SelectedIndex < 0 || SelectedIndex >= craftableItemsList.Count)
            {
                Debug.Log("index out of bound lengh " + craftableItemsList.Count + " index " + SelectedIndex);
                return;
            }
            Debug.Log("use item  " + craftableItemsList[SelectedIndex].name + " index " + craftableItemsList[SelectedIndex].index + " length " + craftableItemsList[SelectedIndex].length);
            var name = craftableItemsList[SelectedIndex].name;
            RemoveFromStack(craftableItemsList[SelectedIndex].index,craftableItemsList[SelectedIndex].length);
            CmdRequestUse(name);
        }

        [Command]
        private void CmdRequestUse(CraftItemName name)
        {
            EventSystems.EventManager.Instance.TriggerEvent<CraftItemData>(new CraftItemData(netIdentity,name));
        }

    }
}