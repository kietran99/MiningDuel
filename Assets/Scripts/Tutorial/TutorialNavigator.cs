using MD.Diggable;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MD.Tutorial
{
    public class TutorialNavigator : MonoBehaviour
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private string mainGameSceneName;

        [SerializeField]
        private GameObject tutorialContainer = null;

        [SerializeField]
        private Text tutorialText = null;

        [SerializeField]
        private Color cantProceedContainerColor = Color.red;

        [SerializeField]
        private GameObject indicator = null;

        [SerializeField]
        private Text skipText = null;

        [SerializeField]
        private GameObject sonar = null, timer = null, scoreCounter = null, digButton = null;

        [SerializeField]
        private string[] lines = null;
        #endregion

        private bool CanProceedNextLine
        {
            get => canProceedNextLine;
            set
            {
                canProceedNextLine = value;
                if (canProceedNextLine) tutorialContainer.GetComponent<Image>().color = Color.white;
                else tutorialContainer.GetComponent<Image>().color = cantProceedContainerColor;                
            }
        }

        private bool canProceedNextLine;

        private int currentLine = 0;

        private enum IndicatorDir { UP, DOWN, LEFT, RIGHT }

        private Dictionary<IndicatorDir, float> dirAngles;

        private float curMoveDist = 0f;
        private Vector2 lastMovePos;
        private readonly float MOVE_DIST_TO_SHOW_SONAR = 5f;

        private readonly int TUT_JOYSTICK_IDX = 6;
        private readonly int TUT_SONAR_IDX = 17;
        private readonly int TUT_MOVE_TO_GEM_IDX = 23;
        private readonly int TUT_SCORE_IDX = 27;
        private readonly int TUT_TIMER_IDX = 37;

        void Start()
        {
            CanProceedNextLine = true;

            dirAngles = new Dictionary<IndicatorDir, float>
            {
                { IndicatorDir.RIGHT, 0f },
                { IndicatorDir.LEFT, 180f },
                { IndicatorDir.DOWN, -90f },
                { IndicatorDir.UP, 90f }
            };
        }

        private void Update()
        {
            if (!CanProceedNextLine) return;

            //if (currentLine >= lines.Length - 1) skipText.text = "Start";

            if (!Input.GetMouseButtonDown(0)) return;

            if (currentLine >= lines.Length - 2)
            {
                skipText.text = "Start";
                return;
            }

            HideIndicator();
            ShowNextLine();
            
            if (currentLine == TUT_JOYSTICK_IDX)
            {
                ShowJoystick();
                EventSystems.EventManager.Instance.StartListening<MoveData>(AccumCurMoveDist);
                return;
            }

            if (currentLine == TUT_JOYSTICK_IDX + 1)
            {
                CanProceedNextLine = false;
                return;
            }

            if (currentLine == TUT_SONAR_IDX)
            {
                sonar.SetActive(true);
                return;
            }

            if (currentLine == TUT_MOVE_TO_GEM_IDX)
            {
                CanProceedNextLine = false;
                EventSystems.EventManager.Instance.StartListening<GemCollideTutorialData>(ShowDigButton);
                return;
            }

            if (currentLine == TUT_SCORE_IDX)
            {
                ShowScoreCounter();                
                return;
            }

            if (currentLine == TUT_TIMER_IDX)
            {
                ShowTimer();
            }
        }

        private void ShowNextLine()
        {           
            currentLine++;
            tutorialText.text = lines[currentLine];
        }

        private void AccumCurMoveDist(MoveData moveData)
        {
            if (!lastMovePos.x.IsEqual(moveData.x) && !lastMovePos.y.IsEqual(moveData.y))
            {
                curMoveDist += Mathf.Abs(lastMovePos.x - moveData.x) + Mathf.Abs(lastMovePos.y - moveData.y);
                lastMovePos = new Vector2(moveData.x, moveData.y);
            }

            if (curMoveDist < MOVE_DIST_TO_SHOW_SONAR) return;
            
            CanProceedNextLine = true;
            ShowNextLine();
            EventSystems.EventManager.Instance.StopListening<MoveData>(AccumCurMoveDist);
        }

        private void ShowDigButton(GemCollideTutorialData gemCollideData)
        {
            digButton.SetActive(true);
            ShowIndicator(IndicatorDir.DOWN, 340f, -74f);
            ShowNextLine();
            EventSystems.EventManager.Instance.StartListening<GemDigSuccessData>(ProceedToScore);
            EventSystems.EventManager.Instance.StopListening<GemCollideTutorialData>(ShowDigButton);                      
        }
       
        private void ProceedToScore(GemDigSuccessData data)
        {
            CanProceedNextLine = true;
            HideIndicator();
            ShowNextLine();
            EventSystems.EventManager.Instance.StopListening<GemDigSuccessData>(ProceedToScore);
        }

        public void SkipTutorial()
        {
            SceneManager.LoadScene(mainGameSceneName);
        }

        private void ShowJoystick()
        {
            ShowIndicator(IndicatorDir.DOWN, -329f, -47f);
        }

        private void ShowScoreCounter()
        {
            scoreCounter.SetActive(true);
            ShowIndicator(IndicatorDir.UP, 330f, 120f);
        }

        private void ShowTimer()
        {
            timer.SetActive(true);
            ShowIndicator(IndicatorDir.UP, 0f, 100f);
        }

        private void ShowIndicator(IndicatorDir dir, float posX, float posY)
        {
            indicator.SetActive(true);
            indicator.transform.Rotate(0f, 0f, dirAngles[dir]);
            indicator.transform.localPosition = new Vector3(posX, posY, indicator.transform.position.z);
        }

        private void HideIndicator()
        {
            indicator.transform.rotation = Quaternion.identity;
            indicator.gameObject.SetActive(false);
        }
    }
}