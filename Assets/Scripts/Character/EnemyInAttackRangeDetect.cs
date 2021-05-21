using System.Collections.Generic;
using UnityEngine;

namespace MD.Character
{
    public class EnemyInAttackRangeDetect : MonoBehaviour
    {
        [SerializeField]
        private Transform pickaxe = null;

        private Dictionary<int, Transform> cachedDamagableDict;
        private List<Transform> trackingTargets;

        public System.Action<bool> OnTrackingTargetsChanged;

        private void Start()
        {
            cachedDamagableDict = new Dictionary<int, Transform>();
            trackingTargets = new List<Transform>();
        }

        public void RaiseAttackDirEvent() 
        {
            var targetAngle = -(pickaxe.localEulerAngles.z + 90f);
            // Debug.Log("Angle: " + targetAngle);
            var atkDir = new Vector2(-Mathf.Cos(Mathf.Deg2Rad * targetAngle), Mathf.Sin(Mathf.Deg2Rad * targetAngle));
            EventSystems.EventManager.Instance.TriggerEvent(new AttackDirectionData(atkDir));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!TryGetTransform(other, out var otherTransform))
            {
                return;
            }

            trackingTargets.Add(otherTransform);

            if (trackingTargets.Count == 1) 
            {
                OnTrackingTargetsChanged?.Invoke(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!TryGetTransform(other, out var otherTransform))
            {
                return;
            }

            trackingTargets.Remove(otherTransform);
            
            if (trackingTargets.Count == 0) 
            {
                OnTrackingTargetsChanged?.Invoke(false);
            }
        }

        private bool TryGetTransform(Collider2D other, out Transform otherTransform)
        {
            if (!cachedDamagableDict.TryGetValue(other.GetInstanceID(), out otherTransform))
            {
                if (!other.TryGetComponent<IDamagable>(out var _))
                {
                    return false;
                } 

                otherTransform = other.transform;
                cachedDamagableDict.Add(other.GetInstanceID(), otherTransform);        
            }

            return true;
        }

        private void Update()
        {
            if (trackingTargets.Count == 0)
            {
                return;
            }

            var closestTarget = trackingTargets.Reduce(GetCloserObject);
            pickaxe.transform.Rotate(0f, 0f, Vector2.SignedAngle(-pickaxe.transform.up, closestTarget.position - pickaxe.transform.position));
        }

        private Transform GetCloserObject(Transform transform_0, Transform transform_1)
        {
            return (transform_0.position - transform.position).sqrMagnitude < (transform_1.position - transform.position).sqrMagnitude ? transform_0 : transform_1;
        }    
    }
}
