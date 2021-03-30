using UnityEngine;
using UnityEngine.UI;

namespace MD.Tutorial
{
    public class TutorialStorage : MonoBehaviour
    {
        private const float TIMES_CHECK = 20;

        [SerializeField]
        private Diggable.Gem.GemValue gem = null;

        [SerializeField]
        private float storeTime = 2f;

        [SerializeField]
        private GameObject ProcessBar = null;

        [SerializeField]
        private Image ProcessBarImage = null;

        private float checkTime;
        private bool isInside;

        private void Start()
        {
            checkTime = storeTime / TIMES_CHECK;
        }

        void OnTriggerEnter2D(Collider2D collide)
        {
            if (!collide.CompareTag(Constants.PLAYER_TAG)) 
            {
                return;
            }

            isInside = true;
            ShowProcessBar();
            StartCoroutine(nameof(StoringScore));
        }

        void OnTriggerExit2D(Collider2D collide)
        {
            if (!collide.CompareTag(Constants.PLAYER_TAG)) 
            {
                return;
            }

            HideProcessBar();
            isInside = false;
        }

        private System.Collections.IEnumerator StoringScore()
        {
            var waitTime = new WaitForSeconds(checkTime);

            for (int i = 1; i <= TIMES_CHECK; i++)
            {
                yield return waitTime;

                if (!isInside) yield break;

                ShowProcess((float)i /TIMES_CHECK);
            }

            EventSystems.EventManager.Instance.TriggerEvent(new Character.FinalScoreChangeData(gem.Value));
            EventSystems.EventManager.Instance.TriggerEvent(new Character.ScoreChangeData(0));
        }

        private void ShowProcess(float amount)
        {
            if (!ProcessBar.activeInHierarchy) ProcessBar.SetActive(true);
            ProcessBarImage.fillAmount = amount;
        }

        private void HideProcessBar()
        {
            ProcessBar.SetActive(false);
        }

        private void ShowProcessBar()
        {
            ProcessBarImage.fillAmount = 0;
            ProcessBar.SetActive(true);
        }
    }
}
