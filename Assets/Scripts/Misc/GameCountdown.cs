using MD.Character;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    [RequireComponent(typeof(Text))]
    public class GameCountdown : MonoBehaviour, IGameCountDown
    {       
        private Text timerText;

        private int currentMin, currentSec;

        private float timeToNextSec;

        private bool gameEnded = false, gameStarted = false;

        private void Awake()
        {
            timerText = GetComponent<Text>();
            ServiceLocator.Register<IGameCountDown>(this);
        }

        private void Start()
        {
            timeToNextSec = 1f;
            (currentMin, currentSec) = GetMinAndSec(timerText.text);
            EventSystems.EventManager.Instance.StartListening<EndGameData>(StopCountDown);
        }
        private void OnDestroy()
        {
            EventSystems.EventManager.Instance.StopListening<EndGameData>(StopCountDown);
        }

        public void StartCountDown(float Time)
        {
            gameStarted = true;
        }

        private void StopCountDown(EndGameData data)
        {
            (currentMin,currentSec) = (0,0);
            UpdateRemainingTime();
        }

        void Update()
        {
// #if UNITY_EDITOR
//             if (Input.GetKeyDown(KeyCode.Q))
//             {
//                 EventSystems.EventManager.Instance.TriggerEvent(new EndGameData(GetCurrentScore()));
//             }
// #endif
            if (!gameStarted || gameEnded) return;

            if (currentMin == 0 && currentSec == 0)
            {
                Debug.Log("Game Over");
                // EventSystems.EventManager.Instance.TriggerEvent(new EndGameData(GetCurrentScore()));
                gameEnded = true;
                return;
            }

            if (timeToNextSec > 0f)
            {
                timeToNextSec -= Time.deltaTime;
                return;
            }
            (currentMin, currentSec) = CalcNextMinAndSec(currentMin, currentSec);
            UpdateRemainingTime();
            timeToNextSec = 1f;
        }

        // private int GetCurrentScore()
        // {
        //     Player player;
        //     if(ServiceLocator.Resolve<Player>(out player))
        //         return player.GetCurrentScore();
        //     return -1;
        // }   

        private (int min, int sec) GetMinAndSec(string time)
        {
            var GetMinAndSec = time.Split(':');           
            return (int.Parse(GetMinAndSec[0]), int.Parse(GetMinAndSec[1]));
        }

        private void UpdateRemainingTime()
        {           
            timerText.text = FormatTime(currentMin) + ":" + FormatTime(currentSec);
        }

        private (int min, int sec) CalcNextMinAndSec(int curMin, int curSec)
            => curSec == 0 ? (--curMin, 59) : (curMin, --curSec);

        private string FormatTime(int minOrSec) => (minOrSec < 10 ? "0" : string.Empty) + minOrSec;
    }
}