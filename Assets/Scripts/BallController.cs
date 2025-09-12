using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Game Over")]
    public float minY = -5f; // límite de altura para perder

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("RB kinematic: " + rb.isKinematic + "  gravity: " + rb.useGravity);

    }

    private void Update()
    {
        // Si la pelota cae demasiado -> Game Over
        if (transform.position.y < minY)
        {
            GameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entró en trigger con: " + other.name);

        if (other.CompareTag("Hoop"))
        {
            HoopSpawner spawner = Object.FindAnyObjectByType<HoopSpawner>();
            spawner.OnBallScored(other.GetComponent<HoopController>());
        }
        if (other.CompareTag("DeathZone"))
        {
            Debug.Log("Zona de muerte detectada");
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! La pelota cayó al vacío.");

        // Resetear score
        ScoreManager.instance.ResetScore();

        // Reiniciar los aros
        HoopSpawner spawner = Object.FindAnyObjectByType<HoopSpawner>();
        if (spawner != null)
        {
            spawner.ResetHoops();
        }

        // Resetear físicas de la pelota
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Volver al aro inicial
        transform.position = new Vector3(0, 2, 0);
    }
}
