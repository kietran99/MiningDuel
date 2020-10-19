using UnityEngine;

namespace MD.Diggable.Projectile
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Generator/Diggable/Projectile/Stats")]
    public class ProjectileStats : ScriptableObject
    {
        [SerializeField]
        private Sprite projectileSprite = null;

        [SerializeField]
        private float stunTime = .5f;

        [SerializeField]
        private float gemDropPercentage = 10;

        public Sprite ProjectileSprite { get => projectileSprite; }

        public float StunTime { get => stunTime; }

        public float GemDropPercentage { get => gemDropPercentage; }
    }
}