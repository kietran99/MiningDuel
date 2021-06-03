using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MD.Diggable.Gem;

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
        private CraftingRecipe recipeSO = null;

        [SerializeField]
        private int MAX_NO_SLOTS = 15;

        [SerializeField]
        private int stackSize = 0;

        [SerializeField]
        private DiggableType[] gemStack = null;
        [SerializeField]
        private int head = 0; //start index of the stack
        private int tail; //last index of the stack ;

        [SerializeField]
        List<CraftableItemsData> craftableItemsList = null;
        
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

            // register for craft material indicator
            ServiceLocator.Register<GemStackManager>(this);
        }

        private void OnDisable()
        {
            EventSystems.EventManager.Instance.StopListening<GemObtainData>(HandleGemObtain);
            EventSystems.EventManager.Instance.StopListening<CraftMenuChangeIndexData>(HandleChangeIndex);
            EventSystems.EventManager.Instance.StopListening<UseItemInvokeData>(HandleUseItemInvoke);
            craftableItemsList.Clear();
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
            for (int i=0 ; i< stackSize - (recipeSO.SHORT_RECIPE_LENGTH -1); i++)
            {
                List<CraftableItemsData> res = CanCraft(GetIndex(i), out bool skipCheck);
                craftableItemsList.AddRange(res);
                if (skipCheck) i+= recipeSO.LONG_RECIPE_LENGTH -1;
            }
            EventSystems.EventManager.Instance.TriggerEvent(new CraftableItemsListChangeData(GetItemListData()));

            EventSystems.EventManager.Instance.TriggerEvent(new GemStackUsedData(GetPos(index),length));
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
            // bool isRemovingfirstGem = false; isChanged = false;
            // if (stackSize == MAX_NO_SLOTS) isRemovingfirstGem = true;
            
            if (!recipeSO.IsGemCraftable(data.type)) return;
            AddToStack(data.type);
            if (stackSize <recipeSO.SHORT_RECIPE_LENGTH) return;

            // List<CraftableItemsData> tempList = new List<CraftableItemsData>();
            craftableItemsList.Clear();
            for (int i = 0; i< stackSize - (recipeSO.SHORT_RECIPE_LENGTH -1) ; i++) // ignore 2 last gems
            {
                List<CraftableItemsData> res = CanCraft(GetIndex(i), out bool skipCheck);
                if (skipCheck) i+= recipeSO.LONG_RECIPE_LENGTH -1;
                craftableItemsList.AddRange(res);
            }

            // craftableItemsList = tempList;
            EventSystems.EventManager.Instance.TriggerEvent(new CraftableItemsListChangeData(GetItemListData()));
            //update Material indicator when add a new gem
            EventSystems.EventManager.Instance.TriggerEvent(new CraftMenuChangeIndexData(Mathf.Max(SelectedIndex))); 
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

        private int GetPos(int index)
        {
            int pos = index - head;
            if (pos < 0) pos += gemStack.Length;
            Debug.Log("pos is " + pos + " index  is " + index);
            return pos;
        }

        private List<CraftableItemsData> CanCraft(int index, out bool skipCheck)
        {
            List<CraftableItemsData> res = new List<CraftableItemsData>();
            // DiggableType[] materials = new DiggableType[recipeSO.MAX_NO_MATERIALS];
            //get materials
            // for (int i = 0; i < recipeSO.MAX_NO_MATERIALS; i++)
            // {
            //     int currentIndex = index  + i;
            //     if (currentIndex >= gemStack.Length) currentIndex -= gemStack.Length;
            //     materials[i] = gemStack[currentIndex]; 
            //     //if end of stack fill the rest with empty
            //     if (currentIndex == tail)
            //     {
            //         for (int j = i+1; j< materials.Length; j++)
            //         {
            //             materials[j] = DiggableType.EMPTY;
            //         }
            //         break;
            //     }
            // }

            // foreach(Recipe recipe in recipeSO.Recipes)
            // {
            //     for (int i = 0; i<= recipe.Materials.Length; i++)
            //     {
            //         if (i == recipe.Materials.Length)
            //         {
            //             itemNames.Add(new CraftableItemsData(recipe.craftItemName,index,recipe.Materials.Length));
            //             break;
            //         }
            //         if (materials[i] == DiggableType.EMPTY || materials[i] != (DiggableType )recipe.Materials[i]) break;
            //     }
            // }
            List<CraftableGem> materials = new List<CraftableGem>();
            int pos = GetPos(index);
            if (pos  <= stackSize - recipeSO.SHORT_RECIPE_LENGTH) //check min recipes
            {
                for (int i = 0; i < recipeSO.SHORT_RECIPE_LENGTH; i++)
                {
                    materials.Add((CraftableGem) gemStack[GetIndex(pos + i)]);
                }
                CraftItemName item = recipeSO.Search(materials.ToArray());
                if (item != CraftItemName.None) res.Add(new CraftableItemsData(item,index,recipeSO.SHORT_RECIPE_LENGTH));
            }
            skipCheck = false;
            if (pos  <= stackSize - recipeSO.LONG_RECIPE_LENGTH) //check min recipes
            {
                for (int i = recipeSO.SHORT_RECIPE_LENGTH; i < recipeSO.LONG_RECIPE_LENGTH; i++)
                {
                    materials.Add((CraftableGem) gemStack[GetIndex(pos + i)]);
                }

                //check if all gem are the same type
                skipCheck = true;
                CraftableGem gem = materials[0];
                for (int i=1; i< materials.Count; i++)
                {
                    if (gem != materials[i])
                    {
                        skipCheck = false;
                        break;
                    }
                } 
                CraftItemName item = recipeSO.Search(materials.ToArray());
                if (item != CraftItemName.None) res.Add(new CraftableItemsData(item,index,recipeSO.LONG_RECIPE_LENGTH));
            }
            return res;
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

        public (int, int) GetCraftItemMaterialsInfor(int index)
        {
            if (index < 0 || index >= craftableItemsList.Count) return (0, 0);
            return (GetPos(craftableItemsList[index].index), craftableItemsList[index].length);
        }

    }
}