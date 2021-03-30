using MD.Character;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField]
        private Text scoreText = null;

        [SerializeField]
        private Text finalScoreText = null;
        [SerializeField] private float scoreUpdateTime = .5f;
        private bool scoreUpdate = false;
        [SerializeField] private float finalScoreUpdateTime = .5f;
        private bool finalScoreUpdate = false;
        private float scoreChange = 0f;
        private float currentScore = 0f;
        private float nextScore = 0f;
        private float finalScoreChange = 0f;
        private float currentFinalScore = 0f;
        private float nextFinalScore = 0f;

        void Start()
        {
            EventSystems.EventManager.Instance.StartListening<ScoreChangeData>(UpdateScoreText);
            EventSystems.EventManager.Instance.StartListening<FinalScoreChangeData>(UpdateFinalScoreText);
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<ScoreChangeData>(UpdateScoreText);
            EventSystems.EventManager.Instance.StopListening<FinalScoreChangeData>(UpdateFinalScoreText);
        }

        public void UpdateScoreText(ScoreChangeData data)
        {

            currentScore = int.Parse(scoreText.text);
            nextScore = data.newScore;
            scoreUpdate = true;
            scoreChange = (nextScore - currentScore)/scoreUpdateTime;
            // scoreText.text = data.newScore.ToString();
        }
        public void UpdateFinalScoreText(FinalScoreChangeData data)
        {
            currentFinalScore = int.Parse(finalScoreText.text);
            nextFinalScore = data.finalScore;
            finalScoreUpdate = true;
            finalScoreChange = (nextFinalScore - currentFinalScore)/finalScoreUpdateTime;
            // finalScoreText.text = data.finalScore.ToString();
        }

        void Update()
        {
            if(scoreUpdate)
            {
                if(((currentScore >= nextScore)&&(scoreChange > 0))
                    ||((currentScore <= nextScore)&&(scoreChange < 0)))
                {
                    scoreUpdate = false;
                    return;
                }
                currentScore += scoreChange*Time.deltaTime;
                scoreText.text = ((int)currentScore).ToString();
                // return;
            }
            if(finalScoreUpdate)
            {
                if(((currentFinalScore >= nextFinalScore)&&(finalScoreChange > 0))
                    ||((currentFinalScore <= nextFinalScore)&&(finalScoreChange < 0)))
                {
                    finalScoreUpdate = false;
                    return;
                }
                currentFinalScore += finalScoreChange*Time.deltaTime;
                finalScoreText.text = ((int)currentFinalScore).ToString();
                // return;
            }
        }
    }
}