using UnityEngine;

public class BallTrailController : MonoBehaviour
{
    private ParticleSystem trailParticles;

    private void Awake()
    {
        trailParticles = GetComponentInChildren<ParticleSystem>(true);
        if (trailParticles != null)
            trailParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void PlayTrail()
    {
        if (trailParticles != null && !trailParticles.isPlaying)
            trailParticles.Play();
    }

    public void StopTrail()
    {
        if (trailParticles != null && trailParticles.isPlaying)
            trailParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
