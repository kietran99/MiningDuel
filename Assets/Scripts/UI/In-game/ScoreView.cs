using MD.Character;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField]
        private Text scoreText = null;

        void Start()
        {
            UpdateScoreText(0);
            EventSystems.EventManager.Instance.StartListening<ScoreChangeData>(HandleScoreChange);
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<ScoreChangeData>(HandleScoreChange);
        }

        private void HandleScoreChange(ScoreChangeData data) => UpdateScoreText(data.newScore);

        public void UpdateScoreText(int score) => scoreText.text = score.ToString();
    }
}