using UnityEngine;
using System;
using System.Collections.Generic;

namespace MD.Character
{
    public class WeaponDamageZone : MonoBehaviour
    {
        [SerializeField]
        protected float arcMeasure = 225f;

        [SerializeField]
        private float speed = 504f;

        [Range(0f, 1f)]
        [SerializeField]
        protected float counterablePercentage = .8f;

        [SerializeField]
        private Transform pivotTransform = null;

        [SerializeField]
        private WeaponCriticalZone criticalZone = null;

        private Quaternion baseRotation;
        protected float rotatedArc = 0f;
        private bool isSwinging = false;
        private HashSet<int> hitList = new HashSet<int>();
        protected float counterableArc;

        public Action<IDamagable, bool> OnDamagableCollide { get; set; }
        public Action<Vector2> OnCounterSuccessfully { get; set; }
        public Action<Vector2> OnGetCountered { get; set; }

        private void Start()
        {
            baseRotation = transform.localRotation;
            counterableArc = counterablePercentage * arcMeasure;
            criticalZone.OnCollide += OnCriticalZoneEnter;
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
                // Debug.Log("Regular Hit: " + other.name);
                hitList.Add(other.GetInstanceID());
                OnDamagableCollide?.Invoke(damagable, false);
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

        private void OnCriticalZoneEnter(Collider2D other)
        { 
            if (hitList.Contains(other.GetInstanceID()))
            {
                return;
            }

            if (!other.TryGetComponent<IDamagable>(out var damagable))
            {
                return;
            }

            // Debug.Log("Critical Hit: " + other.name);
            hitList.Add(other.GetInstanceID());
            OnDamagableCollide?.Invoke(damagable, true);          
        }
    }
}
