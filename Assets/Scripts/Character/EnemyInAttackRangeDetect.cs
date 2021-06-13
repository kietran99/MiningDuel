using System.Collections.Generic;
using UnityEngine;

namespace MD.Character
{
    public class EnemyInAttackRangeDetect : MonoBehaviour
    {
        [SerializeField]
        private GameObject player = null;

        [SerializeField]
        private Transform pickaxe = null;

        private int playerUid;
        private Dictionary<int, Transform> cachedDamagableDict;
        private List<Transform> trackingTargets;
        private Transform lastTarget;
        private bool detectActive = true;

        private void Start()
        {
            playerUid = player.GetComponent<IPlayer>().GetUID();
            cachedDamagableDict = new Dictionary<int, Transform>();
            trackingTargets = new List<Transform>();
            lastTarget = transform;
            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<MainWeaponToggleData>(OnMainWeaponToggle);
        }
        
        private void OnMainWeaponToggle(MainWeaponToggleData data)
        {
            if (detectActive == data.isActive)
            {
                return;
            }
            
            detectActive = data.isActive;
            
            if (!detectActive)
            {
                return;
            }

            var curAction = trackingTargets.Count >= 1 ? MainActionType.ATTACK : MainActionType.DIG; 
            EventSystems.EventManager.Instance.TriggerEvent(new MainActionToggleData(curAction));
        }

        public void RaiseAttackDirEvent() 
        {
            var targetAngle = -(pickaxe.localEulerAngles.z + 90f);
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

            if (!detectActive) 
            {
                return;
            }

            if (trackingTargets.Count == 1)
            {
                EventSystems.EventManager.Instance.TriggerEvent(new Character.MainActionToggleData(MainActionType.ATTACK));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {   
            if (!TryGetTransform(other, out var otherTransform))
            {
                return;
            }
            
            trackingTargets.Remove(otherTransform);
            
            if (!detectActive) 
            {
                return;
            }

            if (trackingTargets.Count == 0) 
            {
                EventSystems.EventManager.Instance.TriggerEvent(new Character.MainActionToggleData(MainActionType.DIG));
                EventSystems.EventManager.Instance.TriggerEvent(new AttackTargetChangeData(playerUid, false, Vector2.zero));
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
            if (!detectActive)
            {
                return;
            }

            if (trackingTargets.Count == 0)
            {
                return;
            }

            var closestTarget = trackingTargets.Reduce(GetCloserObject);
            EventSystems.EventManager.Instance.TriggerEvent(new AttackTargetChangeData(playerUid, true, closestTarget.transform.position));
            pickaxe.transform.Rotate(0f, 0f, Vector2.SignedAngle(-pickaxe.transform.up, closestTarget.position - pickaxe.transform.position));
        }

        private Transform GetCloserObject(Transform transform_0, Transform transform_1)
        {
            return (transform_0.position - transform.position).sqrMagnitude < (transform_1.position - transform.position).sqrMagnitude ? transform_0 : transform_1;
        }    
    }
}
