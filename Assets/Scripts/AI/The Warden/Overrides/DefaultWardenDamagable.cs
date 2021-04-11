using UnityEngine;

namespace MD.AI.TheWarden
{
    public class DefaultWardenDamagable : MonoBehaviour, IWardenDamagable
    {
        public void TakeDamage()
        {
            Debug.Log("DAMAGABLE");
        }
    }
}
