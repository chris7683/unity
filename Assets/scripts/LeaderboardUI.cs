using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    public static LeaderboardUI instance;

    public GameObject leaderboardPanel;
    public Transform entriesContainer;
    public GameObject entryRowPrefab;
    public TMP_Text yourScoreText;
    public TMP_Text statusText;

    void Awake()
    {
        instance = this;
    }

    public void Show(int score)
    {
        if (leaderboardPanel == null)
        {
            Debug.LogError("LeaderboardUI: leaderboardPanel is not assigned.");
            return;
        }

        leaderboardPanel.SetActive(true);
        if (yourScoreText != null) yourScoreText.text = "Score: " + score;
        ClearRows();

        if (LeaderboardManager.instance == null)
        {
            if (statusText != null) statusText.text = "Not logged in";
            return;
        }

        if (statusText != null) statusText.text = "Submitting...";

        LeaderboardManager.instance.SubmitScore(score, () =>
        {
            if (statusText != null) statusText.text = "Loading...";

            // Update best score text on the game over panel after Firebase responds
            LeaderboardManager.instance.GetUserBestScore(best =>
            {
                if (GameManager.instance != null && GameManager.instance.bestScoreText != null)
                    GameManager.instance.bestScoreText.text = "Best: " + best;
                else
                    Debug.LogWarning("bestScoreText not assigned on GameManager.");
            });

            LeaderboardManager.instance.GetTopScores(ShowRows);
        });
    }

    void ShowRows(List<LeaderboardEntry> entries)
    {
        if (statusText != null) statusText.text = "";
        ClearRows();

        if (entriesContainer == null)
        {
            Debug.LogError("LeaderboardUI: entriesContainer is not assigned.");
            return;
        }

        if (entryRowPrefab == null)
        {
            Debug.LogError("LeaderboardUI: entryRowPrefab is not assigned.");
            return;
        }

        string myId = AuthManager.instance != null ? AuthManager.instance.GetUserId() : null;

        foreach (var e in entries)
        {
            var row = Instantiate(entryRowPrefab, entriesContainer);
            var texts = row.GetComponentsInChildren<TMP_Text>(true);

            if (texts.Length >= 3)
            {
                texts[0].text = "#" + e.rank;
                texts[1].text = e.username;
                texts[2].text = e.bestScore.ToString();
            }
            else
            {
                Debug.LogWarning("EntryRow prefab has fewer than 3 TMP_Text children. Found: " + texts.Length);
            }

            if (e.userId == myId)
            {
                var img = row.GetComponent<Image>();
                if (img != null) img.color = new Color(1f, 0.85f, 0f, 0.3f);
            }
        }
    }

    void ClearRows()
    {
        if (entriesContainer == null) return;
        foreach (Transform child in entriesContainer)
            Destroy(child.gameObject);
    }
}