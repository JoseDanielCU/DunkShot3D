using UnityEngine;

public class HoopController : MonoBehaviour
{

    [Header("Disparo")]
    public Transform shootPoint;   // Posición desde donde se disparará
    public float shootForce = 10f;

    private BallController ball;   // Referencia a la pelota
    private bool scored = false;   // Para evitar múltiples scores en un mismo aro

    void Start()
    {
        // Buscar la pelota en la escena (solo habrá una)
        ball = Object.FindAnyObjectByType<BallController>();
    }

    public void ShootBall()
    {
        if (ball == null || shootPoint == null) return;

        // Reposicionar la pelota en el punto de disparo
        ball.transform.position = shootPoint.position;

        // Activar físicas y aplicar fuerza
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (scored) return;
        if (other.CompareTag("Ball"))
        {
            scored = true;

            ScoreManager.instance.AddScore(1);

            HoopSpawner spawner = FindAnyObjectByType<HoopSpawner>();
            if (spawner != null)
                spawner.OnBallScored(this);

            // Permitir nuevo disparo
            FindAnyObjectByType<BallShooter>().canShoot = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Si la bola está dentro del aro
            BallShooter shooter = other.GetComponent<BallShooter>();
            if (shooter != null && other.attachedRigidbody.linearVelocity.magnitude < 0.1f)
            {
                // Reactivar disparo si está quieta dentro del aro
                shooter.canShoot = true;
            }
        }
    }
}

