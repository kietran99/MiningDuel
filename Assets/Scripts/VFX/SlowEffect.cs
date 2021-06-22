namespace MD.VisualEffects
{
    public class SlowEffect : TrailParticle
    {
        public void Play() 
        {
            particle.Play();
        }
        
        public void Stop()
        {
            if (particle.isPlaying)
                particle.Stop();
        }
    }
}