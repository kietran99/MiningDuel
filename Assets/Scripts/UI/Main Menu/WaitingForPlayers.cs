using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class WaitingForPlayers : MonoBehaviour
{
    [SerializeField]
    private float interval = .2f;

    private Text wfpText;
    private string wfp = "Waiting for players";
    private string[] dots = new string[3] { ".", "..", "..." };
    private int curIdx = 0;

    private void Awake()
    {
        wfpText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        curIdx = 0;
        StartCoroutine(Load());
    }

    private void OnDisable()
    {
        StopCoroutine(Load());
    }

    private IEnumerator Load()
    {
        WaitForSecondsRealtime waitSecs = new WaitForSecondsRealtime(interval);

        while (true)
        {
            wfpText.text = wfp + dots[curIdx];

            yield return waitSecs;

            curIdx = curIdx == dots.Length - 1 ? 0 : curIdx + 1;
        }
    }
}
