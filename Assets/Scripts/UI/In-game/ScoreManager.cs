using MD.Diggable;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        private Text scoreText = null;

        private int currentScore;

        public int CurrentScore
        {
            get => currentScore;
            set
            {
                currentScore = value;
                scoreText.text = currentScore.ToString();
            }
        }

        void Start()
        {
            EventSystems.EventManager.Instance.StartListening<GemDigSuccessData>(IncreaseCurrentScore);
            CurrentScore = 0;
        }

        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<GemDigSuccessData>(IncreaseCurrentScore);
        }

        private void IncreaseCurrentScore(GemDigSuccessData data)
        {
            CurrentScore += data.value;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) CurrentScore += 100;
        }
#endif
    }
}