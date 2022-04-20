using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class FirebaseScript : MonoBehaviour
{
    
    [SerializeField] bool willResetLeaderboard;
    FirebaseDatabase database;
    DatabaseReference leaderboardReference;
    DatabaseReference root;
    const string LEADERBOARD = "Leaderboard";
    public delegate void LeaderboardCallback(List<LeaderboardEntry> entries);
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            database = FirebaseDatabase.DefaultInstance;
        });
        leaderboardReference = database.RootReference.Child(LEADERBOARD);
        root = database.RootReference;
        
        if (willResetLeaderboard)
            ResetLeaderboard();
            
    }
    class BoolClass
    {
        public bool isTrue = false;
    }
    class DataSnapshotClassf
    {
        public DataSnapshot dataSnapshot;
    }
    
    private void ResetLeaderboard()
    {
        LeaderboardEntry entry = new LeaderboardEntry("Empty", 0);
        string json = JsonUtility.ToJson(entry);
        for(int i = 0; i < 10; i++)
        {
            leaderboardReference.Child(i.ToString()).SetRawJsonValueAsync(json);
        }
    }
    public void LoadLeaderboard(LeaderboardCallback leaderboardCallback)
    {
        StartCoroutine(LoadLeaderboardCoroutine(leaderboardCallback));
    }
    IEnumerator LoadLeaderboardCoroutine(LeaderboardCallback leaderboardCallback)
    {
        var query = leaderboardReference.GetValueAsync();
        yield return new WaitUntil(() => query.IsCompleted);

        DataSnapshot[] snapshots = query.Result.Children.ToArray();
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
        for(int i = 0; i>10; i++)
        {
            entries.Add(JsonUtility.FromJson<LeaderboardEntry>(snapshots[i].GetRawJsonValue()));
        }
        entries.Sort((x, y) => x.score.CompareTo(y.score));
        leaderboardCallback(entries);
    }
    public struct LeaderboardEntry
    {
        public string name;
        public int score;
        public LeaderboardEntry(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }
    
    public void SubmitFinalScore(string name, int score)
    {
        LeaderboardEntry entry = new LeaderboardEntry(name, score);
        StartCoroutine(SubmitFinalScoreCoroutine(entry));
    }

    IEnumerator SubmitFinalScoreCoroutine(LeaderboardEntry entry)
    {
        
        var query = leaderboardReference.GetValueAsync();
        yield return new WaitUntil(() => query.IsCompleted);

        DataSnapshot[] snapshots = query.Result.Children.ToArray();
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
        for(int i = 0; i < 10; i++)
        {
            entries.Add(JsonUtility.FromJson<LeaderboardEntry>(snapshots[i].GetRawJsonValue()));
        }
        entries.Add(entry);
        entries.Sort((x, y) => x.score.CompareTo(y.score));
        entries.RemoveAt(10);
        int taskCounter = 1;
        for(int i = 0; i<10; i++)
        {
            string json = JsonUtility.ToJson(entries[i]);
            leaderboardReference.SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                taskCounter++;
            });
        }
        yield return new WaitUntil(() => taskCounter == 10);

        SceneManager.LoadScene(LEADERBOARD);
    }
}
