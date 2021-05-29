using UnityEngine;

namespace MD.VisualEffects
{
    public class ExplosionEffectController : MonoBehaviour
    {
        [SerializeField]
        private ObjectPool vfxPrefabPool = null;

        private ObjectPoolCache<ExplosionEffect> poolCache;

        private void Start()
        {
            poolCache = new ObjectPoolCache<ExplosionEffect>(vfxPrefabPool);
            EventSystems.EventConsumer.Attach(gameObject).StartListening<ExplosionEffectRequestData>(PlayEffect);
        }

        private void PlayEffect(ExplosionEffectRequestData data)
        {
            var vfx = poolCache.Pop();
            vfx.transform.parent = null;
            vfx.transform.position = data.pos;
            vfx.Play(() => EffectEndCallback(vfx));
        }

        private void EffectEndCallback(ExplosionEffect vfx)
        {
            vfx.transform.parent = vfxPrefabPool.transform;
            poolCache.Push(vfx);
        }
    }
}
