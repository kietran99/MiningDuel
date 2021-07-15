using UnityEngine;

namespace MD.VisualEffects
{
    public class HitEffectSpawner : MonoBehaviour
    {
        [SerializeField]
        private ObjectPool _vfxPrefabPool = null;

        private ObjectPoolCache<HitEffect> _poolCache;

        void Start()
        {
            _poolCache = new ObjectPoolCache<HitEffect>(_vfxPrefabPool);
            EventSystems.EventConsumer.Attach(gameObject).StartListening<Character.AttackCollideData>(OnAttackCollide);
        }

        private void OnAttackCollide(Character.AttackCollideData data)
        {
            var vfx = _poolCache.Pop(true);
            vfx.transform.position = new Vector3(data.posX, data.posY, 0f);
            vfx.Play(PushBackToPool);
        }

        private void PushBackToPool(HitEffect vfx)
        {
            _poolCache.Push(vfx);
        }
    }
}
