using UnityEngine;

namespace MD.VisualEffects
{
    public class TextPopupSpawner : MonoBehaviour
    {
        [SerializeField]
        private Color _normalDamageColor = Color.white;

        [SerializeField]
        private Color _criticalDamageColor = Color.white;

        [SerializeField]
        private ObjectPool _popupPool = null;

        [SerializeField]
        private GameObject canvas = null;

        private Camera _camera;
        private ObjectPoolCache<TextPopup> _poolCache;

        private void Start()
        {
            _camera = Camera.main;
            _poolCache = new ObjectPoolCache<TextPopup>(_popupPool);
            EventSystems.EventConsumer.Attach(gameObject).StartListening<Character.DamageGivenData>(OnDamageGiven);
        }

        private void OnDamageGiven(Character.DamageGivenData data)
        {
            Spawn(data.dmg, _camera.WorldToScreenPoint(data.damagablePos), data.isCritical);
        }

        private void Spawn(int dmg, Vector2 spawnPos, bool isCritical)
        {
            var popup = _poolCache.Pop();
            popup.transform.SetParent(canvas.transform);
            popup.Play(dmg.ToString(), spawnPos, isCritical ? _criticalDamageColor : _normalDamageColor, PushToPool);
        }

        private void PushToPool(TextPopup popup)
        {
            _poolCache.Push(popup);
        }
    }
}
