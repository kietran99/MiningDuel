using UnityEngine;

namespace MD.VisualEffects
{
    public class HitEffectSpawner : MonoBehaviour
    {
        [SerializeField]
        private ObjectPool _hitEffectPool = null;

        [SerializeField]
        private ObjectPool _criticalHitEffectPool = null;

        private ObjectPoolCache<HitEffect> _hitPoolCache;
        private ObjectPoolCache<HitEffect> _criticalHitPoolCache;

        void Start()
        {
            _hitPoolCache = new ObjectPoolCache<HitEffect>(_hitEffectPool);
            _criticalHitPoolCache = new ObjectPoolCache<HitEffect>(_criticalHitEffectPool);
            EventSystems.EventConsumer.Attach(gameObject).StartListening<Character.AttackCollideData>(OnAttackCollide);
        }

        private void OnAttackCollide(Character.AttackCollideData data)
        {
            var vfx = data.isCritical ? _criticalHitPoolCache.Pop(true) : _hitPoolCache.Pop(true);
            vfx.transform.position = new Vector3(data.posX, data.posY, 0f);
            if (data.isCritical)
            {
                vfx.Play(PushBackToCriticalHitPool);
            }
            else
            {
                vfx.Play(PushBackToHitPool);
            }
        }

        private void PushBackToHitPool(HitEffect vfx)
        {
            _hitPoolCache.Push(vfx);
        }

        private void PushBackToCriticalHitPool(HitEffect vfx)
        {
            _criticalHitPoolCache.Push(vfx);
        }
    }
}
