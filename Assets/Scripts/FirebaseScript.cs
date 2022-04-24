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
using UnityEngine.Events;

public class FirebaseScript : MonoBehaviour
{
    [SerializeField] UnityEvent OnFirebaseConnected = new UnityEvent();
    [SerializeField] bool willResetLeaderboard;
    FirebaseDatabase database;
    DatabaseReference leaderboardReference;
    const string LEADERBOARD = "Leaderboard";
    public delegate void LeaderboardCallback(List<LeaderboardEntry> entries);
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.Log(task.Exception);

            database = FirebaseDatabase.DefaultInstance;
            leaderboardReference = database.RootReference.Child(LEADERBOARD);
            //leaderboardReference.SetValueAsync("leaderboardTest");


            if (willResetLeaderboard)
                ResetLeaderboard();
            OnFirebaseConnected.Invoke();
            //Debug.Log("here");
        });
        
            
    }
   
    
    private void ResetLeaderboard()
    {
        
        LeaderboardEntry entry = new LeaderboardEntry("Empty", 0);
        string json = JsonUtility.ToJson(entry);
        for(int i = 0; i < 10; i++)
        {
            leaderboardReference.Child(i.ToString()).SetRawJsonValueAsync(json);
        }
        Debug.Log("ResetLeaderboard");
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
        for(int i = 0; i<10; i++)
        {
            entries.Add(JsonUtility.FromJson<LeaderboardEntry>(snapshots[i].GetRawJsonValue()));
        }
        entries.Sort((x, y) => x.score.CompareTo(y.score));
        entries.Reverse();
        leaderboardCallback(entries);
    }
    public class LeaderboardEntry
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
        Debug.Log($"snapshots.Length = {snapshots.Length}");
        for (int i = 0; i < 10; i++)
        {
            entries.Add(JsonUtility.FromJson<LeaderboardEntry>(snapshots[i].GetRawJsonValue()));
        }
        entries.Add(entry);
        
        entries.Sort((x, y) => x.score.CompareTo(y.score));
        entries.Reverse();
        entries.RemoveAt(10);
        int taskCounter = 0;
        for(int i = 0; i<10; i++)
        {
            string json = JsonUtility.ToJson(entries[i]);
            Debug.Log($"Entry {i} = {entries[i].name} {entries[i].score}");
            leaderboardReference.Child(i.ToString()).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                taskCounter++;
            });
        }
        yield return new WaitUntil(() => taskCounter == 10);

        SceneManager.LoadScene(LEADERBOARD);
    }
}
