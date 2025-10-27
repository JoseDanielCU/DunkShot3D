using UnityEngine;

public class HoopController : MonoBehaviour
{
    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip rimHitSound;
    public AudioClip[] scoreSounds;

    [Header("Wave")]
    public Material waveMaterial;
    private float waveTimer = 0f;
    private bool waveActive = false;

    [Header("Disparo")]
    public Transform shootPoint;
    public float shootForce = 10f;

    private BallController ball;
    private bool scored = false;

    void Start()
    {
        ball = Object.FindAnyObjectByType<BallController>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // 🔹 Método para marcar el aro inicial como ya anotado
    public void SetAsAlreadyScored() => scored = true;

    public void TriggerWave()
    {
        waveTimer = 0f;
        waveActive = true;
    }

    public void ShootBall()
    {
        if (ball == null || shootPoint == null) return;

        ball.transform.position = shootPoint.position;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
    }

    // 🔸 Sonido cuando la pelota ENTRA al aro (punto)
    private void OnTriggerEnter(Collider other)
    {
        if (scored) return;

        if (other.CompareTag("Ball"))
        {
            scored = true;

            // ✅ Apagar estela
            BallTrailController trail = other.GetComponent<BallTrailController>();
            if (trail != null)
                trail.StopTrail();

            // ✅ Sonido de anotación
            // 🔹 Sonido de anotación aleatorio y dinámico
            if (scoreSounds != null && scoreSounds.Length > 0 && audioSource != null)
            {
                int randomIndex = Random.Range(0, scoreSounds.Length);
                AudioClip chosenClip = scoreSounds[randomIndex];

                audioSource.pitch = Random.Range(0.95f, 1.05f); // variación ligera del tono
                audioSource.PlayOneShot(chosenClip);
            }


            // ✅ Sumar puntaje
            ScoreManager.instance.AddScore(1);

            // ✅ Efecto visual
            TriggerWave();

            // ✅ Notificar al spawner
            HoopSpawner spawner = FindAnyObjectByType<HoopSpawner>();
            if (spawner != null)
                spawner.OnBallScored(this);

            // ✅ Permitir nuevo disparo
            FindAnyObjectByType<BallShooter>().canShoot = true;
        }
    }

    // 🔸 Sonido cuando la pelota GOLPEA el aro metálico
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            if (audioSource != null && rimHitSound != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f); // variación sutil
                audioSource.PlayOneShot(rimHitSound);
            }
        }
    }

    void Update()
    {
        if (!waveActive) return;

        waveTimer += Time.deltaTime;
        waveMaterial.SetFloat("_WaveTime", waveTimer);

        // Desactivar efecto después de un tiempo
        if (waveTimer > 2f)
            waveActive = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        BallShooter shooter = other.GetComponent<BallShooter>();
        BallTrailController trail = other.GetComponent<BallTrailController>();

        // Apagar estela si la bola está quieta dentro del aro
        if (trail != null && rb.linearVelocity.magnitude < 0.1f)
            trail.StopTrail();

        // Permitir nuevo disparo
        if (shooter != null && rb.linearVelocity.magnitude < 0.1f)
            shooter.canShoot = true;
    }
}
