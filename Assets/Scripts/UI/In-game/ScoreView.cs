using MD.Character;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class ScoreView : MonoBehaviour
    {
        class ScoreUIIbject
        {
            public Text UIText;
            public bool changing = false;
            public int score = 0;
            public int target = 0;

            public ScoreUIIbject(Text UIText)
            {
                this.UIText = UIText;
            }
        }

        [SerializeField]
        private Text scoreText = null;

        [SerializeField]
        private Text finalScoreText = null;

        [SerializeField]
        private float delayTime = .03f;
        
        ScoreUIIbject currentScore;
        ScoreUIIbject finalScore;

        void Start()
        {
            EventSystems.EventManager.Instance.StartListening<ScoreChangeData>(UpdateScoreText);
            EventSystems.EventManager.Instance.StartListening<FinalScoreChangeData>(UpdateFinalScoreText);
            currentScore = new ScoreUIIbject(scoreText);
            finalScore = new ScoreUIIbject(finalScoreText);
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<ScoreChangeData>(UpdateScoreText);
            EventSystems.EventManager.Instance.StopListening<FinalScoreChangeData>(UpdateFinalScoreText);
        }

        public void UpdateScoreText(ScoreChangeData data)
        {
            currentScore.target = data.newScore;
            if (!currentScore.changing)
                StartCoroutine(ChangeScoreGradually(currentScore));
        }
        public void UpdateFinalScoreText(FinalScoreChangeData data)
        {
            finalScore.target = data.finalScore;
            if (!finalScore.changing)
                StartCoroutine(ChangeScoreGradually(finalScore));
        }

        private System.Collections.IEnumerator ChangeScoreGradually(ScoreUIIbject scoreUIIbject)
        {
            scoreUIIbject.changing = true;
            int target = scoreUIIbject.target;
            var delay = new WaitForSeconds(delayTime);
            if (target > scoreUIIbject.score)
            {
                while (scoreUIIbject.score < target)
                {
                    scoreUIIbject.score++;
                    scoreUIIbject.UIText.text = scoreUIIbject.score.ToString();
                    yield return delay;
                }
            }
            else
            {
                while (scoreUIIbject.score > target)
                {
                    scoreUIIbject.score--;
                    scoreUIIbject.UIText.text = scoreUIIbject.score.ToString();
                    yield return delay;
                }
            }
            if (scoreUIIbject.score != scoreUIIbject.target)
            {
                StartCoroutine(ChangeScoreGradually(scoreUIIbject));
            }
            else
                scoreUIIbject.changing = false;
        }

    }

}