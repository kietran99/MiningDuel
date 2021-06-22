using UnityEngine;

namespace MD.VisualEffects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class TrailParticle : MonoBehaviour
    {
        protected ParticleSystem particle;
        Vector2 lastPos = Vector3.zero;

        protected void OnEnable()
        {   
            particle = GetComponent<ParticleSystem>();
        }

        protected void Update()
        {
            if (!particle.isPlaying) 
            {
                return;
            }

            if (lastPos == (Vector2)transform.position)
            {
                return;
            }
           
            float angle = Vector2.SignedAngle(Vector2.up, (Vector2)transform.position - lastPos);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            var main = particle.main;   
            main.startRotationZ = -angle * Mathf.Deg2Rad;
            lastPos = transform.position;     
        }
    }
}