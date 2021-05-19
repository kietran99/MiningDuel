using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD.Diggable.Gem;
public class GemStackUI : MonoBehaviour
{
    [SerializeField]
    private int MAX_NO_SLOTS = 15;

    [SerializeField]
    private RectTransform[] SlotsRect;

    [SerializeField]
    private GameObject SuperRareGemObjectPoolPrefab;

    [SerializeField]
    private GameObject RareGemObjectPoolPrefab;
    
    [SerializeField]
    private GameObject UncommonGemObjectPoolPrefab;

    [SerializeField]
    private GameObject CommonGemObjectPoolPrefab;


    
    private GameObject[] Slots;

    private int count;

    private IObjectPool SuperRareGemPool;
    private IObjectPool RareGemPool;

    private IObjectPool UncommonGemPool;

    private IObjectPool CommonGemPool;
    

    private void Awake()
    {
        InitializePool();
        Initialize();
    }

    private void Initialize()
    {
        Slots = new GameObject[MAX_NO_SLOTS];
    }

    private void Start()
    {
        var consumer = GetComponent<EventSystems.EventConsumer>();
        consumer.StartListening<GemObtainData>(AddNewGem);
        consumer.StartListening<GemStackUsedData>(RemoveGem);
        count = 0;
    }

    private void InitializePool()
    {
        SuperRareGemPool = Instantiate(SuperRareGemObjectPoolPrefab,Vector3.zero,Quaternion.identity,gameObject.transform).GetComponent<IObjectPool>();
        RareGemPool = Instantiate(RareGemObjectPoolPrefab,Vector3.zero,Quaternion.identity,gameObject.transform).GetComponent<IObjectPool>();
        UncommonGemPool = Instantiate(UncommonGemObjectPoolPrefab,Vector3.zero,Quaternion.identity,gameObject.transform).GetComponent<IObjectPool>();
        CommonGemPool = Instantiate(CommonGemObjectPoolPrefab,Vector3.zero,Quaternion.identity,gameObject.transform).GetComponent<IObjectPool>();
    }

    private void AddNewGem(GemObtainData data)
    {
        GameObject newGem = GetSlotObject(data.type);
        if (newGem == null) return; 
        if (count < MAX_NO_SLOTS)
        {
            Slots[count] = newGem;
            Slots[count].transform.position = SlotsRect[count].position;
            count++;
            return;
        }

        //full
        DiscardSlotObject(Slots[0].gameObject);
        for (int i=0 ; i< MAX_NO_SLOTS; i++)
        {
            //add new gem if last slot
            if (i == MAX_NO_SLOTS - 1)
            {
                Slots[i] = newGem;
                Slots[i].transform.position = SlotsRect[i].position;
                return;
            }
            Slots[i] = Slots[i+1];
            Slots[i].transform.position = SlotsRect[i].position;
        }
    }

    private void RemoveGem(GemStackUsedData data)
    {
        for (int i = data.pos ; i < data.pos + data.length ; i++)
        {
            DiscardSlotObject(Slots[i]);
        }

        for (int i = data.pos ; i < count - data.length; i++)
        {
            Slots[i] = Slots[i + data.length];
            Slots[i + data.length] = null;
            Slots[i].transform.position = SlotsRect[i].position;
        }
        count-=data.length;
    }

    private void DiscardSlotObject(GameObject obj)
    {
        SuperRareGemPool.Push(obj);
        RareGemPool.Push(obj);
        UncommonGemPool.Push(obj);
        CommonGemPool.Push(obj);
    }

    private GameObject GetSlotObject(DiggableType type)
    {
        switch(type)
        {
            case DiggableType.SUPER_RARE_GEM:
                return SuperRareGemPool.Pop();
            case DiggableType.RARE_GEM:
                return RareGemPool.Pop();
            case DiggableType.UNCOMMON_GEM:
                return UncommonGemPool.Pop();
            case DiggableType.COMMON_GEM:
                return CommonGemPool.Pop();
            default:
                return null;
        }
    }

}
