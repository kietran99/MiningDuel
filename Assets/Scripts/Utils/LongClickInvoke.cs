using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Events;

public class LongClickInvoke : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool mouseHold;
    private float ptrDownTimer = 0f;

    [SerializeField]
    private float requiredHoldTime = 0f;

    [SerializeField]
    private UnityEvent onLongClick = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseHold = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
        mouseHold = false;
    }

    void Update()
    {
        if (!mouseHold) return;

        ptrDownTimer += Time.deltaTime;

        if (ptrDownTimer < requiredHoldTime) return;

        onLongClick?.Invoke();

        Reset();
    }

    private void Reset()
    {
        ptrDownTimer = 0f;
    }
}
