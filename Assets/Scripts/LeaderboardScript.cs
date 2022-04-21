using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardScript : MonoBehaviour
{
    [SerializeField] FirebaseScript firebaseScript;
    [SerializeField] TMP_Text[] leaderboardTexts;
    public void SetLeaderboard()
    {
        Debug.Log("SetLeaderboard");
        firebaseScript.LoadLeaderboard(SetLeaderboardTexts);
    }
    void SetLeaderboardTexts(List<FirebaseScript.LeaderboardEntry> entries)
    {
        for (int i = 0; i<10; i++)
        {
            leaderboardTexts[i].text = $" {i+1}. {entries[i].name} {entries[i].score}";
        }
    }
}
