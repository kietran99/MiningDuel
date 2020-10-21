using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    [RequireComponent(typeof(Text))]
    public class GameCountdown : MonoBehaviour
    {
        [SerializeField]
        private ScoreManager scoreManager = null;

        private Text timerText;

        private int currentMin, currentSec;

        private float timeToNextSec;

        private bool gameEnded = false;

        private void Awake()
        {
            timerText = GetComponent<Text>();
        }

        private void Start()
        {
            timeToNextSec = 1;
            (currentMin, currentSec) = GetMinAndSec(timerText.text);
        }

        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Q))
            {
                EventSystems.EventManager.Instance.TriggerEvent(new EndGameData(scoreManager.CurrentScore));
            }
#endif
            if (gameEnded) return;

            if (currentMin == 0 && currentSec == 0)
            {
                Debug.Log("Game Over");
                EventSystems.EventManager.Instance.TriggerEvent(new EndGameData(scoreManager.CurrentScore));
                gameEnded = true;
                return;
            }

            if (timeToNextSec > 0f)
            {
                timeToNextSec -= Time.deltaTime;
                return;
            }

            UpdateRemainingTime();
            timeToNextSec = 1f;
        }

        private (int min, int sec) GetMinAndSec(string time)
        {
            var GetMinAndSec = time.Split(':');           
            return (int.Parse(GetMinAndSec[0]), int.Parse(GetMinAndSec[1]));
        }

        private void UpdateRemainingTime()
        {           
            (currentMin, currentSec) = CalcNextMinAndSec(currentMin, currentSec);
            timerText.text = FormatTime(currentMin) + ":" + FormatTime(currentSec);
        }

        private (int min, int sec) CalcNextMinAndSec(int curMin, int curSec)
            => curSec == 0 ? (--curMin, 59) : (curMin, --curSec);

        private string FormatTime(int minOrSec) => (minOrSec < 10 ? "0" : string.Empty) + minOrSec;
    }
}