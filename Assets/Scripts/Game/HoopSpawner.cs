using UnityEngine;
using System.Collections.Generic;

public class HoopSpawner : MonoBehaviour
{
    public GameObject hoopPrefab;
    private HoopController currentHoop;
    private HoopController nextHoop;

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
    }

    private HoopController SpawnNewHoop()
    {
        // Usar el DifficultyManager para obtener la posición
        Vector3 newPos;
        
        if (DifficultyManager.instance != null)
        {
            newPos = DifficultyManager.instance.GetRandomHoopPosition(currentHoop.transform.position);
        }
        else
        {
            // Fallback si no hay DifficultyManager
            newPos = new Vector3(
                Random.Range(-2f, 2f),
                Random.Range(1f, 3f),
                currentHoop.transform.position.z + Random.Range(4f, 6f)
            );
        }

        HoopController newHoop = Instantiate(hoopPrefab, newPos, Quaternion.Euler(90, 0, 0))
                                 .GetComponent<HoopController>();

        spawnedHoops.Add(newHoop);

        // Agregar movimiento si es necesario
        if (DifficultyManager.instance != null && DifficultyManager.instance.ShouldHoopMove())
        {
            HoopMovement movement = newHoop.gameObject.AddComponent<HoopMovement>();
            movement.Initialize(
                DifficultyManager.instance.GetMoveSpeed(),
                DifficultyManager.instance.GetMoveRange()
            );
        }

        // Agregar obstáculo si es necesario
        if (DifficultyManager.instance != null && DifficultyManager.instance.ShouldSpawnObstacle())
        {
            SpawnObstacle(currentHoop.transform.position, newPos);
        }

        return newHoop;
    }

    private void SpawnObstacle(Vector3 fromPos, Vector3 toPos)
    {
        if (DifficultyManager.instance.obstaclePrefab == null) return;

        // Calcular dirección hacia el siguiente aro
        Vector3 direction = (toPos - fromPos).normalized;
        
        // Posición más cerca del próximo aro (70% del camino)
        Vector3 obstaclePos = fromPos + direction * Vector3.Distance(fromPos, toPos) * 0.7f;
        
        // Colocar a la altura del aro objetivo
        obstaclePos.y = toPos.y;

        GameObject obstacle = Instantiate(DifficultyManager.instance.obstaclePrefab, obstaclePos, Quaternion.identity);
        
        // Rotar 90 grados para que sea una pared horizontal perpendicular al tiro
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);
        obstacle.transform.rotation = Quaternion.LookRotation(perpendicular);
        
        // Escalar para hacer una pared más grande
        obstacle.transform.localScale = new Vector3(3f, 2f, 0.2f); // Pared ancha y alta, pero delgada
        
        // Agregar a la lista
        spawnedObstacles.Add(obstacle);
        
        // Destruir después de un tiempo
        Destroy(obstacle, 30f);
    }

    public void OnBallScored(HoopController hoop)
    {
        if (hoop == nextHoop)
        {
            Destroy(currentHoop.gameObject);
            spawnedHoops.Remove(currentHoop);

            currentHoop = nextHoop;
            nextHoop = SpawnNewHoop();

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

        // Destruir todos los obstáculos
        foreach (var obstacle in spawnedObstacles)
        {
            if (obstacle != null)
                Destroy(obstacle);
        }
        spawnedObstacles.Clear();

        // Resetear dificultad
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