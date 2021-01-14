using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    [RequireComponent(typeof(Text))]
    public class WaitingForPlayer : MonoBehaviour
    {
        [SerializeField]
        private float interval = .2f;

        private Text wfpText;
        private string wfp = "Waiting for player";
        private string[] dots = new string[4] { string.Empty, ".", "..", "..." };
        private int curIdx = 0;

        private void OnEnable()
        {
            wfpText = GetComponent<Text>();
        }

        public void ShowWaitingForPlayer()
        {           
            if (!gameObject.activeInHierarchy) return; 
            
            curIdx = 0;          
            //StartCoroutine(Load());
            wfpText.text = wfp;
        }

        public void ShowPlayerName(string playerName)
        {
            if (!gameObject.activeInHierarchy) return; 

            //StopCoroutine(Load());
            wfpText.text = playerName;
        }

        private IEnumerator Load()
        {
            var waitSecs = new WaitForSecondsRealtime(interval);

            while (true)
            {
                wfpText.text = wfp + dots[curIdx];

                yield return waitSecs;

                curIdx = curIdx == dots.Length - 1 ? 0 : curIdx + 1;
            }
        }
    }
}
