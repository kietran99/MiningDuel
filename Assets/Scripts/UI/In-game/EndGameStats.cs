using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndGameStats : MonoBehaviour
{
    #region SERIALIZE FIELDS
    [SerializeField]
    private GameObject mask = null;

    [SerializeField]
    private GameObject endGameStatsContainer = null;

    [SerializeField]
    private Text scoreText = null;

    [SerializeField]
    private Text winText = null;
    #endregion

    private int score;

    private const float INCREASE_SPEED = .01f;

    private void Start()
    {
        EventSystems.EventManager.Instance.StartListening<EndGameData>(Show);
    }

    private void OnDestroy()
    {
        EventSystems.EventManager.Instance.StopListening<EndGameData>(Show);
    }

    void Update()
    {        
        if (Input.GetMouseButtonDown(0)) ImmediatelyShowScore();
    }

    private void Show(EndGameData endGameData)
    {
        mask.SetActive(true);
        endGameStatsContainer.SetActive(true);
        score = endGameData.score;
        winText.text = endGameData.hasWon ? "Victory" : "Defeated";
        ShowScore(endGameData.score);
    }
    
    private void ShowScore(int score)
    { 
        StartCoroutine(IncreaseScoreGradually(score));
    }

    private IEnumerator IncreaseScoreGradually(int score)
    {
        var delay = new WaitForSecondsRealtime(INCREASE_SPEED);

        for (int i = 0; i <= score; i++)
        {
            yield return delay;

            scoreText.text = i.ToString();
        }
    }

    private void ImmediatelyShowScore()
    {
        StopAllCoroutines();
        scoreText.text = score.ToString();
    }
}
