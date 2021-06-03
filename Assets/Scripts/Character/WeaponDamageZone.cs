using UnityEngine;
using System;
using System.Collections.Generic;

namespace MD.Character
{
    public class WeaponDamageZone : MonoBehaviour
    {
        [SerializeField]
        private float arcMeasure = 225f;

        [SerializeField]
        private float speed = 720f;      

        [SerializeField]
        private Transform pivotTransform = null;

        [SerializeField]
        private WeaponCriticalZone criticalZone = null;

        private Quaternion baseRotation;
        private bool isSwinging = false;
        protected float attackTime = 0f;
        private HashSet<int> hitList = new HashSet<int>();

        public Action<IDamagable, bool> OnDamagableCollide { get; set; }
        public Action<Vector2> OnCounterSuccessfully { get; set; }
        public Action<Vector2> OnGetCountered { get; set; }

        private void Start()
        {
            baseRotation = transform.localRotation;
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
            var rotated = 0f;

            while (rotated < arcMeasure)
            {
                var rotateAngle = Time.deltaTime * speed;
                attackTime += Time.deltaTime;
                transform.RotateAround(pivotTransform.position, -transform.forward, rotateAngle);
                rotated += rotateAngle;

                yield return null;
            }

            Reset();
        }

        private void Reset()
        {
            transform.localRotation = baseRotation;
            isSwinging = false;
            attackTime = 0f;
            hitList = new HashSet<int>();
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
            
            if (!other.TryGetComponent<WeaponDamageZone>(out var otherWeapon))
            {
                return;
            }  
            // Debug.Log(other.name + ": " + otherWeapon.AttackTime + " " + name + ": " + AttackTime);
            if (!Counterable(otherWeapon))
            {
                Reset();
                return;
            }

            var counterVect = (other.transform.position - transform.position).normalized;
            otherWeapon.GetCountered(counterVect);
            OnCounterSuccessfully?.Invoke(counterVect);
        }

        private bool Counterable(WeaponDamageZone otherWeapon) => otherWeapon.attackTime < attackTime;

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

        private void GetCountered(Vector2 counterVect)
        {
            Debug.Log("Get Countered: " + name);
            OnGetCountered?.Invoke(counterVect);
        }
    }
}
