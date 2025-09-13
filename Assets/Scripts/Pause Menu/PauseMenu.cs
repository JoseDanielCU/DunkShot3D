using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance; // opcional si quieres acceder globalmente
    public GameObject pauseMenuUI;
    public bool isPaused = false;

    // Estructura para guardar datos de cada rigidbody
    private class RigidbodyData
    {
        public Rigidbody rb;
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }

    private List<RigidbodyData> rigidbodiesData = new List<RigidbodyData>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        Resume(); 
    }
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;

        // Guardar y congelar todos los rigidbodies de la escena
        rigidbodiesData.Clear();
        Rigidbody[] allRigidbodies = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);

        foreach (Rigidbody rb in allRigidbodies)
        {
            RigidbodyData data = new RigidbodyData
            {
                rb = rb,
                velocity = rb.linearVelocity,
                angularVelocity = rb.angularVelocity
            };

            rigidbodiesData.Add(data);

            rb.isKinematic = true; // congelar movimiento
        }

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;

        // Restaurar todos los rigidbodies guardados
        foreach (RigidbodyData data in rigidbodiesData)
        {
            if (data.rb != null)
            {
                data.rb.isKinematic = false;
                data.rb.linearVelocity = data.velocity;
                data.rb.angularVelocity = data.angularVelocity;
            }
        }

        rigidbodiesData.Clear();
    }

    public void TogglePause()
    {
        if (!isPaused) Pause();
        else Resume();
    }
    public void LoadMenu(string MenuName)
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(MenuName); 
    }
}