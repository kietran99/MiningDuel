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

        void Start()
        {
            UpdateScoreText(0,0);
            EventSystems.EventManager.Instance.StartListening<ScoreChangeData>(HandleScoreChange);
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<ScoreChangeData>(HandleScoreChange);
        }

        private void HandleScoreChange(ScoreChangeData data) => UpdateScoreText(data.newScore, data.finalScore);

        public void UpdateScoreText(int score, int finalScore)
        {
            // finalScoreText.text = finalScore.ToString();
            scoreText.text = score.ToString();
        }
    }
}