using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BallTrailController : MonoBehaviour
{
    private TrailRenderer trail;
    public bool isPreview = false; // 🔹 Nuevo flag para modo preview

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    public void PlayTrail()
    {
        if (trail == null) return;
        trail.emitting = true;
    }

    public void StopTrail()
    {
        if (trail == null) return;
        trail.emitting = false;
        trail.Clear();
    }

    private void Update()
    {
        if (isPreview)
        {
            // 🔹 Hace que el trail se actualice aunque el objeto esté quieto
            trail.time = 2f;
            trail.emitting = true;
        }
    }
}
