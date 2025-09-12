using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TMP_Text scoreText;

    private int score = -1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateUI();
    }

    public void ResetScore()
    {
        score = -1;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}
