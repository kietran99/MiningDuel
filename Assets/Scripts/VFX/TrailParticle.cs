using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MD.VisualEffects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class TrailParticle : MonoBehaviour
    {
        protected ParticleSystem particle;
        Vector2 LastPos = Vector3.zero;

        // Update is called once per frame

        protected void OnEnable()
        {   
            particle = GetComponent<ParticleSystem>();
        }

        protected void Update()
        {
            if (!particle.isPlaying) return;
            if (LastPos != (Vector2)transform.position)
            {
                float angle = Vector2.SignedAngle(Vector2.up,(Vector2)transform.position - LastPos);
                transform.rotation = Quaternion.Euler(0,0,angle);
                var main = particle.main;   
                main.startRotationZ = -angle*Mathf.Deg2Rad;
                LastPos = transform.position;
            }
        }
    }
}