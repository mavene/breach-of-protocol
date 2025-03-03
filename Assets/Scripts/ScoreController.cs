using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreController : Singleton<ScoreController>
{
    // Events
    public UnityEvent<int> scoreChange;

    [System.NonSerialized]
    private int score = 0; // Hide from inspector

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    public void UpdateScore(int value)
    {
        score += value;
        scoreChange.Invoke(score); // Calls ScoreChange (UI)
    }

    public void ResetScore()
    {
        score = 0;
        scoreChange.Invoke(score); // Calls ScoreChange (UI)
    }
}
