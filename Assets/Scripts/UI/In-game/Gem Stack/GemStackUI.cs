﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD.Diggable.Gem;
using UnityEngine.UI;
public class GemStackUI : MonoBehaviour
{

    [SerializeField]
    private int MAX_NO_SLOTS = 15;

    [SerializeField]
    private RectTransform gemsAppearedPos = null;

    [SerializeField]
    private Sprite SuperRareGem= null;

    [SerializeField]
    private Sprite RareGem = null;
    
    [SerializeField]
    private Sprite UncommonGem = null;

    [SerializeField]
    private Sprite CommonGem = null;

    [SerializeField]
    private GameObject GemSlotObjectPool = null;

    [SerializeField]
    private float gemUIObjectWidth = 56f;

    [SerializeField]
    private float gemsMoveTime = .05f;
    [SerializeField]
    private float gemDestroyTime = .2f;

    [SerializeField]
    private float afterEffectTime = .5f;
    [SerializeField]
    private float afterEffectdistance = 15f;


    
    private GameObject[] Slots;
    private Dictionary<int,Vector3> SlotPositionsDict;

    private int count;

    private IObjectPool GemSlotPool;

    private bool needWait = false;

    private Queue<(IEnumerator,bool)> coroutinesQueue;

    private Vector3 basePosition;

    private void Awake()
    {
        InitializePool();
        Initialize();
    }

    private void Initialize()
    {
        Slots = new GameObject[MAX_NO_SLOTS];
        InitializeSlotPosition();
        coroutinesQueue = new Queue<(IEnumerator,bool)>();
    }

    private void Start()
    {
        var consumer = GetComponent<EventSystems.EventConsumer>();
        consumer.StartListening<GemObtainData>(AddNewGem);
        consumer.StartListening<GemStackUsedData>(RemoveGem);
        count = 0;
        basePosition = transform.position;
        StartCoroutine(CoroutineCoordinator());
    }

    private void InitializePool()
    {
        GemSlotPool = Instantiate(GemSlotObjectPool,Vector3.zero,Quaternion.identity,gameObject.transform).GetComponent<IObjectPool>();
        
    }

    private void InitializeSlotPosition()
    {
        SlotPositionsDict = new Dictionary<int, Vector3>(); //2 extras : for (index: -1) and (index: MAX_NO_SLOTS)
        RectTransform parentRect = GetComponent<RectTransform>();
        float yPos = parentRect.position.y;
        for (int i=-1; i<= MAX_NO_SLOTS; ++i)
        {
            float xPos = parentRect.position.x + gemUIObjectWidth/2f + i*gemUIObjectWidth;
            // Debug.Log(i + " " + xPos);
            SlotPositionsDict.Add(i,new Vector3(xPos,yPos,0f));
        }
    }

    private Vector3 GetSlotPosition(int i)
    {
        if (!SlotPositionsDict.TryGetValue(i, out Vector3 value))
        {
            Debug.LogError("invalid i " + i);
            return Vector3.zero;
        }
        return value;
    }

    private void AddNewGem(GemObtainData data)
    {
        GameObject newGem = GetSlotObject(data.type);
        newGem.transform.position = gemsAppearedPos.position;
        if (newGem == null) return; 
        if (count < MAX_NO_SLOTS)
        {
            Slots[count] = newGem;
            // Slots[count].transform.position = SlotsRect[count].position;
            // StartCoroutine(MoveGemEffect(newGem, count, gemsMoveTime*(MAX_NO_SLOTS - count)));
            coroutinesQueue.Enqueue((MoveGemEffect(newGem, count, gemsMoveTime*(MAX_NO_SLOTS - count)),false));
            count++;
            return;
        }
        //full
        GameObject[] movingGems = new GameObject[MAX_NO_SLOTS + 1];
        GameObject discardGem = Slots[0];
        for (int i=0 ; i< MAX_NO_SLOTS; i++)
        {
            //add new gem if last slot
            if (i == MAX_NO_SLOTS - 1)
            {
                movingGems[i] = Slots[i];
                Slots[i] = newGem;
                // Slots[i].transform.position = SlotsRect[i].position;
                break;
            }
            movingGems[i] = Slots[i];
            Slots[i] = Slots[i+1];
            // Slots[i].transform.position = SlotsRect[i].position;
        }
        movingGems[movingGems.Length -1] = newGem;
        // StartCoroutine(AddGemWhenFullEffect(newGem,movingGems, discardGem));
        coroutinesQueue.Enqueue((AddGemWhenFullEffect(newGem,movingGems, discardGem),true));
    }

    private IEnumerator MoveGemEffect(GameObject gem, int end, float time)
    {
        float distance = Vector2.Distance((Vector2) gem.transform.position, (Vector2) GetSlotPosition(end));
        float speed = 0;
        float accelaration = 2*distance / (time*time);
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            yield return null;
            elapsedTime+= Time.deltaTime;
            speed += accelaration*Time.deltaTime;
            gem.transform.position += Vector3.left*speed*Time.deltaTime;
        }
        gem.transform.position = GetSlotPosition(end);
        StartCoroutine(StackAfterEffect());
    }

    private IEnumerator StackAfterEffect(GameObject objectToDiscard = null)
    {
        float elapsedTime = 0f;
        float time = afterEffectTime;
        if (objectToDiscard != null)
        {
            time*=2f;
        }
        float speed = afterEffectdistance/time;
        while (elapsedTime < time)
        {
            yield return null;
            transform.position += Vector3.left*speed*Time.deltaTime;
            elapsedTime += Time.deltaTime;
        }
        if (objectToDiscard != null)
        {
            DiscardSlotObject(objectToDiscard);
        }
        elapsedTime = 0;
        while (elapsedTime < time)
        {
            yield return null;
            transform.position += Vector3.right*speed*Time.deltaTime;
            elapsedTime += Time.deltaTime;
        }
        transform.position = basePosition;
    }

    private IEnumerator MoveMultipleGemsEffect(GameObject[] gems, int startIndex, int IndexMove, float time, GameObject discardObj = null, float waitTime =0f)
    {
        if (waitTime >0) yield return new WaitForSeconds(waitTime);

        float distance = gemUIObjectWidth*IndexMove;
        float speed = 0;
        float accelaration = 2*distance / (time*time);
        float elapsedTime = 0f;
        Vector3 distanceMoved = Vector3.zero;
        while (elapsedTime < time)
        {
            yield return null;
            elapsedTime+= Time.deltaTime;
            speed += accelaration*Time.deltaTime;
            distanceMoved = Vector3.left*Time.deltaTime*speed;
            for (int i=0; i< gems.Length; ++i)
            {
                gems[i].transform.position += distanceMoved;
            }
        }

        for (int i=0; i< gems.Length; ++i)
        {
            gems[i].transform.position = GetSlotPosition(startIndex + i- IndexMove);
        }
        needWait = false;
        StartCoroutine(StackAfterEffect(discardObj));
    }
    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            if (coroutinesQueue.Count >0 && !needWait)
            {
                (IEnumerator,bool) res = coroutinesQueue.Dequeue();
                if (res.Item2 == true) needWait = true;
                yield return StartCoroutine(res.Item1);
            }    
            yield return null;
        }
    }
    private IEnumerator AddGemWhenFullEffect(GameObject newGem,GameObject[] movingGems, GameObject discardGem)
    {
        StartCoroutine( MoveGemEffect(newGem,MAX_NO_SLOTS,gemsMoveTime));
        yield return new WaitForSeconds (gemsMoveTime);
        StartCoroutine( MoveMultipleGemsEffect(movingGems,0,1,gemsMoveTime*3,discardGem));
    }

    private void RemoveGem(GemStackUsedData data)
    {
        Debug.Log("remove data " + data.pos + " " + data.length);
        for (int i = data.pos ; i < data.pos + data.length ; i++)
        {
            DiscardSlotObject(Slots[i]);
        }
        List<GameObject> movingGems = new List<GameObject>();
        for (int i = data.pos ; i < count - data.length; i++)
        {
            Slots[i] = Slots[i + data.length];
            movingGems.Add(Slots[i]);
            // Slots[i + data.length] = null;
            // Slots[i].transform.position = GetSlotPosition(i);
        }
        if (count > (data.pos + data.length))
            coroutinesQueue.Enqueue((MoveMultipleGemsEffect(movingGems.ToArray(), data.pos + data.length, data.length,gemsMoveTime*data.length,null, gemDestroyTime),true));
        count-=data.length;  
    }

    private void DiscardSlotObject(GameObject obj)
    {
        GemSlotPool.Push(obj);
    }

    private GameObject GetSlotObject(DiggableType type)
    {
        GameObject gem = GemSlotPool.Pop();
        Image gemImage = gem.GetComponent<Image>();
        switch(type)
        {
            case DiggableType.SUPER_RARE_GEM:
                gemImage.sprite = SuperRareGem;
                break;
            case DiggableType.RARE_GEM:
                gemImage.sprite = RareGem;
                break;
            case DiggableType.UNCOMMON_GEM:
                gemImage.sprite = UncommonGem;
                break;
            case DiggableType.COMMON_GEM:
                gemImage.sprite = CommonGem;
                break;
        }
        return gem;
    }

}

