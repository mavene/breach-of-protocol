using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public TextMeshProUGUI inGameScoreText;
    public TextMeshProUGUI gameOverScoreText;
    public PersistentIntVariable highScore; // Reference to Scriptable Object for high score

    private int score = 0; // Tracks the current session score

    private void Start()
    {
        LoadHighScore(); // Load previous high score on start
    }

    public void UpdateScore(int value)
    {
        score += value;
        inGameScoreText.text = "Score: " + score.ToString();

        // Update high score if the new score is greater
        if (score > highScore.value)
        {
            highScore.SetValue(score);
            highScore.Save();
        }
    }

    public void UpdateScore5()
    {
        score += 3;
        inGameScoreText.text = "Score: " + score.ToString();

        if (score > highScore.value)
        {
            highScore.SetValue(score);
            highScore.Save();
        }
    }

    public void FinaliseScore()
{
    // Display both final score and high score
    gameOverScoreText.text = "Final Score: " + score.ToString() + 
                             "\nHigh Score: " + highScore.value.ToString();

    Debug.Log($"Game Over! Final Score: {score}, High Score: {highScore.value}");
}

    public void ResetScore()
    {
        score = 0;
        inGameScoreText.text = "Score: 0";
    }

    public void LoadHighScore()
    {
        // Display stored high score when game starts
        Debug.Log("Loaded High Score: " + highScore.value);
    }

    public void ResetHighScore()
    {
        highScore.SetValue(0);
        highScore.Save();
        Debug.Log("High Score Reset!");
    }
}
