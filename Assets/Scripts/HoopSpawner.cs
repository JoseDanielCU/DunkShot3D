using UnityEngine;
using System.Collections.Generic;

public class HoopSpawner : MonoBehaviour
{
    public GameObject hoopPrefab;
    private HoopController currentHoop;
    private HoopController nextHoop;

    private List<HoopController> spawnedHoops = new List<HoopController>();

    void Start()
    {
        InitHoops();
    }

    private void InitHoops()
    {
        spawnedHoops.Clear();

        currentHoop = Instantiate(hoopPrefab, new Vector3(0, 2, 0), Quaternion.Euler(90, 0, 0))
                      .GetComponent<HoopController>();
        spawnedHoops.Add(currentHoop);

        nextHoop = SpawnNewHoop();
    }

    private HoopController SpawnNewHoop()
    {
        Vector3 newPos = new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(1f, 3f),
            currentHoop.transform.position.z + Random.Range(4f, 6f) 
        );


        HoopController newHoop = Instantiate(hoopPrefab, newPos, Quaternion.Euler(90, 0, 0))
                                 .GetComponent<HoopController>();

        spawnedHoops.Add(newHoop);
        return newHoop;
    }

    public void OnBallScored(HoopController hoop)
    {
        if (hoop == nextHoop)
        {
            Destroy(currentHoop.gameObject);
            spawnedHoops.Remove(currentHoop);

            currentHoop = nextHoop;
            nextHoop = SpawnNewHoop();
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

        InitHoops();
    }

    // Propiedades públicas para la cámara
    public HoopController CurrentHoop => currentHoop;
    public HoopController NextHoop => nextHoop;
}
