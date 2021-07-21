using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialImageSwapper : MonoBehaviour
    {
        [SerializeField]
        private GameObject _materialContainer = null;

        [SerializeField]
        private float _transitionSpeed = 1000f;

        [SerializeField]
        private RectTransform _screen = null;

        private System.Collections.Generic.List<GameObject> _materialList;
        private int _curIdx;
        private GameObject _curMaterial;
        private bool _isTransitionPlaying;

        void OnEnable()
        {
            _materialList = new System.Collections.Generic.List<GameObject>(_materialContainer.transform.childCount);
            foreach (Transform child in _materialContainer.transform)
            {
                _materialList.Add(child.gameObject);
            }

            _curIdx = 0;
            _curMaterial = _materialList[_curIdx];
            _curMaterial.SetActive(true);
            _isTransitionPlaying = false;
        }

        public void NextMaterial() // Inspector
        {
            if (_isTransitionPlaying)
            {
                return;
            }

            _curIdx = _curIdx == _materialList.Count - 1 ? 0 : _curIdx + 1;
            _curMaterial = ShowMaterial(_curMaterial, _materialList[_curIdx]);
        }

        public void PrevMaterial() // Inspector
        {
            if (_isTransitionPlaying)
            {
                return;
            }

            _curIdx = _curIdx == 0 ? _materialList.Count - 1 : _curIdx - 1;
            _curMaterial = ShowMaterial(_curMaterial, _materialList[_curIdx], true);
        }

        private GameObject ShowMaterial(GameObject curMaterial, GameObject nextMaterial, bool leftToRight = false)
        {
            StartCoroutine(Transition(curMaterial.transform, nextMaterial.transform, _screen.rect.width, leftToRight));
            return nextMaterial;
        }       

        private System.Collections.IEnumerator Transition(
            Transform flyOutTransform, 
            Transform flyInTransform, 
            float dist, 
            bool leftToRight = false)
        {
            var movedDist = 0f;
            var dirMult = leftToRight ? -1 : 1;
            var flyInDest = flyOutTransform.position;
            var flyOutDest = flyOutTransform.position + new Vector3(-dirMult * dist, 0f, 0f);

            flyInTransform.transform.position = 
                new Vector3(
                    flyOutTransform.transform.position.x + dirMult * dist, 
                    flyInTransform.transform.position.y, 
                    flyInTransform.transform.position.z);

            flyInTransform.gameObject.SetActive(true);

            _isTransitionPlaying = true;

            while (Mathf.Abs(movedDist) < Mathf.Abs(dist))
            {
                var moveDelta = _transitionSpeed * Time.deltaTime;
                
                flyInTransform.Translate(-dirMult * moveDelta, 0f, 0f);
                flyOutTransform.Translate(-dirMult * moveDelta, 0f, 0f);
                movedDist += moveDelta;

                yield return null;
            }

            flyInTransform.transform.position = flyInDest;
            flyOutTransform.transform.position = flyOutDest;
            _isTransitionPlaying = false;
            flyOutTransform.gameObject.SetActive(false);      
        }

        public void CloseTutorial() => gameObject.SetActive(false); // Inspector
    }
}
