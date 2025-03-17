using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreController : Singleton<ScoreController>
{
    // ScriptableObjects
    public IntVariable gameScore;

    // Events
    public UnityEvent<int> scoreChange;
    public UnityEvent<int> highScoreChange;

    // Start is called before the first frame update
    void Start()
    {
        gameScore.Value = 0;
    }

    // Update is called once per frame

    public void UpdateScore(int value)
    {
        gameScore.ApplyChange(value);
        scoreChange.Invoke(gameScore.Value); // Calls ScoreChange (UI)
    }

    public void UpdateHighscore()
    {
        highScoreChange.Invoke(gameScore.previousHighestValue); // Calls HighscoreChange (UI)
    }

    public void ResetScore()
    {
        gameScore.SetValue(0);
        scoreChange.Invoke(gameScore.Value); // Calls ScoreChange (UI)
    }
}
