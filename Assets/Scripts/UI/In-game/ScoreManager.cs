using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class ScoreManager : MonoBehaviour, IScoreManager
    {
        [SerializeField]
        private Text scoreText = null;

        // private int currentScore;

        // public int CurrentScore
        // {
        //     get => currentScore;
        //     set
        //     {
        //         currentScore = value;
        //         scoreText.text = currentScore.ToString();
        //     }
        // }

        void Start()
        {
            // EventSystems.EventManager.Instance.StartListening<GemDigSuccessData>(IncreaseCurrentScore);
            UpdateScoreText(0);
            ServiceLocator.Register<IScoreManager>(this);
        }

        // private void OnDestroy()
        // {
        //     EventSystems.EventManager.Instance.StopListening<GemDigSuccessData>(IncreaseCurrentScore);
        // }

        // private void IncreaseCurrentScore(GemDigSuccessData data)
        // {
        //     CurrentScore += data.value;
        // }

// #if UNITY_EDITOR
//         private void Update()
//         {
//             if (Input.GetKeyDown(KeyCode.Alpha1)) CurrentScore += 100;
//         }
// #endif

        // public int GetCurrentScore()
        // {
        //     return currentScore;
        // }
        // public void DecreaseScore(int score)
        // {
        //     currentScore -= score;
        //     currentScore = currentScore<0 ? 0 : currentScore;
        //     UpdateScoreText();
        // }
        // public void IncreaseScore(int score)
        // {
        //     currentScore += score;
        //     UpdateScoreText();
        // }

        public void UpdateScoreText(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}