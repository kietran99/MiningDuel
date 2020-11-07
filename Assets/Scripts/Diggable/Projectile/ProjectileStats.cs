using MD.Diggable.Core;
using UnityEngine;

namespace MD.Diggable.Projectile
{
    [CreateAssetMenu(fileName = "Projectile Stats", menuName = "Generator/Diggable/Projectile/Stats")]
    public class ProjectileStats : ScriptableObject, Core.IDiggable
    {
        #region SERIALIZE FIELDS
        [SerializeField]
        private Sprite projectileSprite = null;

        [SerializeField]
        private Sprite sonarSprite = null;

        [SerializeField]
        private float stunTime = .5f;

        [SerializeField]
        private float gemDropPercentage = 10;
        #endregion

        public int DigValue { get => 1; }

        public Sprite WorldSprite { get => projectileSprite; }

        public float StunTime { get => stunTime; }

        public float GemDropPercentage { get => gemDropPercentage; }
        public Sprite SonarSprite { get => sonarSprite; }
    }
}