using UnityEngine;

namespace MD.Quirk
{
    [CreateAssetMenu(fileName="Quirk Mapper", menuName="Generator/Quirk/Quirk Mapper")]
    public class QuirkMapper : ScriptableObject
    {
        [System.Serializable]
        public class MapEntry
        {
            public QuirkType type;
            public GameObject prefab;
        }

        [SerializeField]
        private MapEntry[] quirkTable = null;

        private void OnEnable()
        {
            int nonQuirkIdx = quirkTable.LookUp(entry => entry.prefab.GetComponent<BaseQuirk>() == null).idx;
            if (!nonQuirkIdx.Equals(Constants.INVALID))
            {
                Debug.LogError("QUIRK MAPPER: Entry at index: " + nonQuirkIdx + " does not have any BaseQuirk script attached");
            }
        }

        public GameObject Map(QuirkType type)
        {
            return quirkTable.LookUp(entry => entry.type.Equals(type)).item.prefab;
        }
    }  
}