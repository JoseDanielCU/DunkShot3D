using UnityEngine;
using System.Collections.Generic;

public class HoopSpawner : MonoBehaviour
{
    public GameObject hoopPrefab;
    private HoopController currentHoop;
    private HoopController nextHoop;
    public CameraHoopView cameraView;

    private List<HoopController> spawnedHoops = new List<HoopController>();
    private List<GameObject> spawnedObstacles = new List<GameObject>(); // Lista de obstáculos

    void Start()
    {
        InitHoops();
    }

    private void InitHoops()
    {
        spawnedHoops.Clear();

        currentHoop = Instantiate(hoopPrefab, new Vector3(0, 2, 0), Quaternion.Euler(90, 0, 0))
                      .GetComponent<HoopController>();

        // Marcar el primer aro como ya anotado para que no cuente
        currentHoop.SetAsAlreadyScored();

        spawnedHoops.Add(currentHoop);

        nextHoop = SpawnNewHoop();

        if (cameraView != null)
            cameraView.UpdateCameraTarget(true);
    }

    private HoopController SpawnNewHoop()
    {
        // Obtener posición desde DifficultyManager
        Vector3 newPos;
        if (DifficultyManager.instance != null)
            newPos = DifficultyManager.instance.GetRandomHoopPosition(currentHoop.transform.position);
        else
            newPos = new Vector3(Random.Range(-2f, 2f), Random.Range(1f, 3f), currentHoop.transform.position.z + Random.Range(4f, 6f));

        HoopController newHoop = Instantiate(hoopPrefab, newPos, Quaternion.Euler(90, 0, 0))
                                 .GetComponent<HoopController>();

        spawnedHoops.Add(newHoop);

        // 🔹 Solo el "siguiente" aro puede moverse
        if (DifficultyManager.instance != null && DifficultyManager.instance.ShouldHoopMove())
        {
            HoopMovement movement = newHoop.gameObject.AddComponent<HoopMovement>();
            movement.Initialize(
                DifficultyManager.instance.GetMoveSpeed(),
                DifficultyManager.instance.GetMoveRange()
            );
        }

        // Agregar obstáculo si corresponde
        if (DifficultyManager.instance != null && DifficultyManager.instance.ShouldSpawnObstacle())
        {
            SpawnObstacle(currentHoop.transform.position, newPos);
        }

        return newHoop;
    }

    private void SpawnObstacle(Vector3 fromPos, Vector3 toPos)
    {
        if (DifficultyManager.instance.obstaclePrefab == null) return;

        Vector3 direction = (toPos - fromPos).normalized;
        Vector3 obstaclePos = fromPos + direction * Vector3.Distance(fromPos, toPos) * 0.7f;
        obstaclePos.y = toPos.y;

        GameObject obstacle = Instantiate(DifficultyManager.instance.obstaclePrefab, obstaclePos, Quaternion.identity);

        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);
        obstacle.transform.rotation = Quaternion.LookRotation(perpendicular);
        obstacle.transform.localScale = new Vector3(3f, 2f, 0.2f);

        spawnedObstacles.Add(obstacle);
        Destroy(obstacle, 30f);
    }

    public void OnBallScored(HoopController hoop)
    {
        if (hoop == nextHoop)
        {
            // 🔹 Detener el movimiento del aro actual (por si tenía HoopMovement)
            HoopMovement currentMovement = currentHoop.GetComponent<HoopMovement>();
            if (currentMovement != null)
            {
                Destroy(currentMovement);
            }

            // 🔹 Eliminar el aro anterior
            Destroy(currentHoop.gameObject);
            spawnedHoops.Remove(currentHoop);

            // Actualizar referencias
            currentHoop = nextHoop;
            nextHoop = SpawnNewHoop();

            // 🔹 Solo el nuevo "nextHoop" puede moverse. 
            // Aseguramos que el actual (donde la pelota cayó) esté quieto.
            HoopMovement newCurrentMovement = currentHoop.GetComponent<HoopMovement>();
            if (newCurrentMovement != null)
            {
                Destroy(newCurrentMovement);
            }

            // Actualizar cámara
            if (cameraView != null)
                cameraView.UpdateCameraTarget();

            // Verificar si aumentar dificultad
            if (DifficultyManager.instance != null)
            {
                DifficultyManager.instance.CheckDifficultyIncrease();
            }
        }
    }

    public void ResetHoops()
    {
        foreach (var hoop in spawnedHoops)
        {
            if (hoop != null)
                Destroy(hoop.gameObject);
        }
        spawnedHoops.Clear();

        foreach (var obstacle in spawnedObstacles)
        {
            if (obstacle != null)
                Destroy(obstacle);
        }
        spawnedObstacles.Clear();

        if (DifficultyManager.instance != null)
        {
            DifficultyManager.instance.ResetDifficulty();
        }

        InitHoops();
    }

    // Propiedades públicas para la cámara
    public HoopController CurrentHoop => currentHoop;
    public HoopController NextHoop => nextHoop;
}
