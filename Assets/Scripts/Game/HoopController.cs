using UnityEngine;

public class HoopController : MonoBehaviour
{

    [Header("Disparo")]
    public Transform shootPoint;   
    public float shootForce = 10f;

    private BallController ball;   
    private bool scored = false;   

    void Start()
    {
        ball = Object.FindAnyObjectByType<BallController>();
    }

    // Método para marcar el aro inicial como ya anotado
    public void SetAsAlreadyScored()
    {
        scored = true;
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

            FindAnyObjectByType<BallShooter>().canShoot = true;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            BallShooter shooter = other.GetComponent<BallShooter>();
            if (shooter != null && other.attachedRigidbody.linearVelocity.magnitude < 0.1f)
            {
                shooter.canShoot = true;
            }
        }
    }
}