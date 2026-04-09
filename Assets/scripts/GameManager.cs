using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameOverPanel;
    public TMP_Text levelText;

    private int level = 1;
    private SpikeGenerator spikeGenerator;
    private float levelTimer = 15f;
    private float timer;
    private bool isGameOver = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        spikeGenerator = FindFirstObjectByType<SpikeGenerator>();
        timer = levelTimer;
        gameOverPanel.SetActive(false);
        UpdateLevelText();
    }

    void Update()
    {
        if (isGameOver) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            level++;
            spikeGenerator.NextLevel();
            timer = levelTimer;
            UpdateLevelText();
        }
    }

    void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = "Level: " + level;
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}