using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameOverPanel;
    public TMP_Text levelText;
    public TMP_Text bestScoreText;  // public so LeaderboardUI can set it
    public TMP_Text currentScoreText;

    private int level = 1;
    private int score = 0;
    private SpikeGenerator spikeGenerator;
    private float levelTimer = 8f;
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

    public void AddScore(int amount) => score += amount;

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);

        if (currentScoreText != null) currentScoreText.text = "Score: " + score;

        if (LeaderboardUI.instance != null)
            LeaderboardUI.instance.Show(score);
        else
            Debug.LogError("LeaderboardUI.instance is null — is LeaderboardUI on an always-active GameObject?");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Logout()
    {
        Time.timeScale = 1f;
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("login");
    }
}