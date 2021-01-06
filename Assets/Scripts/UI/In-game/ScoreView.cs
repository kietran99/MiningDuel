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

// #if UNITY_EDITOR
//         private void Update()
//         {
//             if (Input.GetKeyDown(KeyCode.Alpha1)) CurrentScore += 100;
//         }
// #endif

        private void HandleScoreChange(ScoreChangeData data) //=> UpdateScoreText(data.newScore);
        {
            Debug.Log("Score View: " + data.newScore);
            UpdateScoreText(data.newScore);
        }

        public void UpdateScoreText(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}