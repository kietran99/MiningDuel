using UnityEngine;

namespace MD.Map.Core
{
    public class MapTester : MonoBehaviour
    {
        void Start()
        {
            IMap map = new DefaultMap();
            try
            {
                map.SetAt(0, 0, null);
            }
            catch (InvalidTileException)
            {
                Debug.LogError("Invalid tile");
            }
        }
    }
}