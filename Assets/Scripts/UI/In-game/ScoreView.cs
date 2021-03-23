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
            scoreText.text = data.newScore.ToString();
        }
        public void UpdateFinalScoreText(FinalScoreChangeData data)
        {
            finalScoreText.text = data.finalScore.ToString();
        }

    }
}