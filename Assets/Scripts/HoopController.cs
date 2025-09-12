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
        if (!other.CompareTag("Ball")) return;

        scored = true;

        // Sumar score y mostrar popup
        int points = 1; // o calcula según net/hoop más adelante
        ScoreManager.instance.AddScore(points, transform.position);

        // Avisar al spawner
        HoopSpawner spawner = FindAnyObjectByType<HoopSpawner>();
        if (spawner != null)
            spawner.OnBallScored(this);
    }

}