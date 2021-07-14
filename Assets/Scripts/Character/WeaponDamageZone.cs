using UnityEngine;
using System;
using System.Collections.Generic;

namespace MD.Character
{
    public class WeaponDamageZone : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField]
        private float criticalRate = .2f;

        [Range(0f, 1f)]
        [SerializeField]
        protected float counterablePercentage = .5f;

        [SerializeField]
        protected float arcMeasure = 225f;

        [SerializeField]
        private float swingAnimDuration = .39f;

        [SerializeField]
        private CircleCollider2D userCollider = null;

        [SerializeField]
        private Transform pivotTransform = null;

        private float speed = 576f;
        private Quaternion baseRotation;
        protected float rotatedArc = 0f;
        private bool isSwinging = false;
        private HashSet<int> hitList = new HashSet<int>();
        protected float counterableArc, minSqrCritDist, maxSqrCritDist;

        public Action<IDamagable, bool> OnDamagableCollide { get; set; }
        public Action<Vector2> OnCounterSuccessfully { get; set; }
        public Action<Vector2> OnGetCountered { get; set; }
        
        private void Start()
        {
            speed = arcMeasure / swingAnimDuration;
            baseRotation = transform.localRotation;
            counterableArc = counterablePercentage * arcMeasure;

            var weaponCollider = GetComponent<BoxCollider2D>();
            var weaponColliderSizeX = weaponCollider.size.x;
            var scale = transform.localScale.x;
            var userColliderRadius = userCollider.radius;   
            var weaponHiltToUserColliderDist = (weaponCollider.offset.x - weaponColliderSizeX * .5f) * scale - userColliderRadius;
            var weaponTipToUserColliderDist = weaponHiltToUserColliderDist + weaponColliderSizeX * scale;
            var maxCritRate = (userColliderRadius * 2) / (weaponTipToUserColliderDist);

            if (criticalRate > maxCritRate)
            {
                Debug.LogWarning($"Critical Rate should be less than or equal to {maxCritRate}");
            }

            var critAreaRadius = (criticalRate * weaponTipToUserColliderDist) * .5f;
            var minCritDist = userColliderRadius + weaponTipToUserColliderDist - critAreaRadius;
            minSqrCritDist = Mathf.Pow(minCritDist, 2f);
            var maxCritDist = userColliderRadius + weaponTipToUserColliderDist + critAreaRadius;
            maxSqrCritDist = Mathf.Pow(maxCritDist, 2f);
            // Debug.Log($"Crit Radius: {critAreaRadius} Min: {minSqrCritDist} Max: {maxSqrCritDist}");
        }

        public void AttemptSwing()
        {
            if (isSwinging)
            {
                return;
            }
            
            gameObject.SetActive(true);
            StartCoroutine(Rotate());
        }

        private System.Collections.IEnumerator Rotate()
        {
            isSwinging = true;

            while (rotatedArc < arcMeasure)
            {
                var rotateAngle = Time.deltaTime * speed;
                transform.RotateAround(pivotTransform.position, -transform.forward, rotateAngle);
                rotatedArc += rotateAngle;

                yield return null;
            }

            Reset();
        }

        private void Reset()
        {
            transform.localRotation = baseRotation;
            rotatedArc = 0f;
            isSwinging = false;
            hitList.Clear();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        { 
            if (hitList.Contains(other.GetInstanceID()))
            {
                return;
            }

            if (other.TryGetComponent<IDamagable>(out var damagable))
            {
                hitList.Add(other.GetInstanceID());
                var sqrDist = (other.transform.position - transform.position).sqrMagnitude;
                // Debug.Log($"Sqr Dist: {sqrDist}");
                var isCritical = sqrDist >= minSqrCritDist && sqrDist <= maxSqrCritDist;
                OnDamagableCollide?.Invoke(damagable, isCritical);
                return;
            }
            
            CounterCheck(other);
        }

        private void CounterCheck(Collider2D other)
        {
            if (!other.TryGetComponent<WeaponDamageZone>(out var otherWeapon))
            {
                return;
            }  

            // Debug.Log(name + ": " + rotatedArc + " " + other.name + ": " + otherWeapon.rotatedArc);

            if (rotatedArc >= counterableArc || otherWeapon.rotatedArc < counterableArc)
            {
                return;
            }

            var counterVect = (other.transform.position - transform.position).normalized;
            // Debug.Log("Get Countered: " + otherWeapon.name);
            otherWeapon.Reset();
            otherWeapon.OnGetCountered?.Invoke(counterVect);
            OnCounterSuccessfully?.Invoke(counterVect);
        }
    }
}
