using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;
    FirebaseFirestore db;
    bool isReady = false;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                isReady = true;
                Debug.Log("LeaderboardManager: Firestore ready.");
            }
            else
            {
                Debug.LogError("LeaderboardManager: Firebase not available — " + task.Result);
            }
        });
    }

    public void InitUser(string userId, string username)
    {
        if (!isReady) return;
        db.Collection("leaderboard").Document(userId).SetAsync(new Dictionary<string, object>
        {
            { "username", username },
            { "bestScore", 0 }
        });
    }

    public void SubmitScore(int score, Action onDone = null)
    {
        if (!isReady || AuthManager.instance == null) { onDone?.Invoke(); return; }
        string uid = AuthManager.instance.GetUserId();
        if (uid == null) { onDone?.Invoke(); return; }

        var doc = db.Collection("leaderboard").Document(uid);
        doc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null) { onDone?.Invoke(); return; }
            int current = task.Result.Exists ? task.Result.GetValue<int>("bestScore") : 0;
            if (score > current)
                doc.UpdateAsync("bestScore", score).ContinueWithOnMainThread(_ => onDone?.Invoke());
            else
                onDone?.Invoke();
        });
    }

    public void GetUserBestScore(Action<int> onResult)
    {
        if (!isReady || AuthManager.instance == null) { onResult?.Invoke(0); return; }
        string uid = AuthManager.instance.GetUserId();
        if (uid == null) { onResult?.Invoke(0); return; }

        db.Collection("leaderboard").Document(uid).GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.Exception != null || !task.Result.Exists) { onResult?.Invoke(0); return; }
                int best = task.Result.ContainsField("bestScore") ? task.Result.GetValue<int>("bestScore") : 0;
                onResult?.Invoke(best);
            });
    }

    public void GetTopScores(Action<List<LeaderboardEntry>> onResult)
    {
        if (!isReady) { onResult?.Invoke(new List<LeaderboardEntry>()); return; }
        db.Collection("leaderboard").OrderByDescending("bestScore").Limit(10)
            .GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogError("GetTopScores failed: " + task.Exception);
                    onResult?.Invoke(new List<LeaderboardEntry>());
                    return;
                }
                var list = new List<LeaderboardEntry>();
                int rank = 1;
                foreach (var doc in task.Result.Documents)
                {
                    list.Add(new LeaderboardEntry
                    {
                        rank = rank++,
                        username = doc.ContainsField("username") ? doc.GetValue<string>("username") : "Unknown",
                        bestScore = doc.ContainsField("bestScore") ? doc.GetValue<int>("bestScore") : 0,
                        userId = doc.Id
                    });
                }
                onResult?.Invoke(list);
            });
    }
}

public class LeaderboardEntry
{
    public int rank;
    public string username;
    public int bestScore;
    public string userId;
}