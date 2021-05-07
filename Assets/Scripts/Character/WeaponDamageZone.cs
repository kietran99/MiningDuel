using UnityEngine;
using System;

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

        private Quaternion baseRotation;
        private bool isSwinging = false;

        public Action<Mirror.NetworkIdentity> OnDamagableCollide { get; set; }
        public Action<Vector2> OnGetCountered { get; set; }

        public float AttackTime { get; set; } = 0f;

        private void Start()
        {
            baseRotation = transform.localRotation;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IDamagable>(out var damagable))
            {
                // Debug.Log("Hit: " + other.name);
                OnDamagableCollide?.Invoke(other.GetComponent<Mirror.NetworkIdentity>());
                return;
            }

            if (!other.TryGetComponent<WeaponDamageZone>(out var weapon))
            {
                return;
            }  
            // Debug.Log(other.name + ": " + weapon.AttackTime + " " + name + ": " + AttackTime);
            if (weapon.AttackTime >= AttackTime)
            {
                Reset();
                return;
            }

            var counterDir = (other.transform.position - transform.position).normalized;
            weapon.GetCounter(counterDir);
            EventSystems.EventManager.Instance.TriggerEvent(new CounterSuccessData(counterDir));
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
                AttackTime += Time.deltaTime;
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
            AttackTime = 0f;
            gameObject.SetActive(false);
        }

        public void GetCounter(Vector2 counterVect)
        {
            Debug.Log("Get Countered: " + name);
            OnGetCountered?.Invoke(counterVect);
        }
    }
}
