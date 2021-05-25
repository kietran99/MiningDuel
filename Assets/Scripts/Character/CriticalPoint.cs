using UnityEngine;
using Mirror;

namespace MD.Character
{
    public class CriticalPoint : MonoBehaviour
    {
        [SerializeField]
        private CircleCollider2D damagableCollider = null;

        public CircleCollider2D DamagableCollider { get => damagableCollider; }
    }
}
