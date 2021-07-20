using UnityEngine;

namespace MD.Tutorial
{
    public class TutorialImageSwapper : MonoBehaviour
    {
        [SerializeField]
        private GameObject _materialContainer = null;

        private System.Collections.Generic.List<GameObject> _materialList;
        private int _curIdx;
        private GameObject curMaterial;

        void OnEnable()
        {
            _materialList = new System.Collections.Generic.List<GameObject>(_materialContainer.transform.childCount);
            foreach (Transform child in _materialContainer.transform)
            {
                _materialList.Add(child.gameObject);
            }

            _curIdx = 0;
            curMaterial = _materialList[_curIdx];
            curMaterial.SetActive(true);
        }

        public void NextMaterial() // Inspector
        {
            _curIdx = _curIdx == _materialList.Count - 1 ? 0 : _curIdx + 1;
            curMaterial = ShowMaterial(curMaterial, _materialList[_curIdx]);
        }

        public void PrevMaterial() // Inspector
        {
            _curIdx = _curIdx == 0 ? _materialList.Count - 1 : _curIdx - 1;
            curMaterial = ShowMaterial(curMaterial, _materialList[_curIdx]);
        }

        private GameObject ShowMaterial(GameObject curMaterial, GameObject nextMaterial)
        {
            curMaterial.SetActive(false);
            nextMaterial.SetActive(true);
            return nextMaterial;
        }       

        public void CloseTutorial() => gameObject.SetActive(false); // Inspector
    }
}
