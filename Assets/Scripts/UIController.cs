using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // ScriptableObjects
    public GameConstants gameConstants;

    // UI components
    public CanvasGroup inGameUI;
    public CanvasGroup gameOverUI;
    public TextMeshProUGUI inGameScoreText;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI highscoreText;
    public Image healthDisplay;
    public Sprite[] healthStates;

    // Ensure Game Over UI components not shown
    private void Start()
    {
        SetCanvasGroupState(gameOverUI, false);
    }

    private void SetCanvasGroupState(CanvasGroup group, bool visibility)
    {
        group.alpha = visibility ? 1 : 0;
        group.interactable = visibility;
        group.blocksRaycasts = visibility;
    }

    public void ScoreChange(int score)
    {
        inGameScoreText.text = "Score: " + score.ToString();
        gameOverScoreText.text = inGameScoreText.text;
    }

    public void HighscoreChange(int score)
    {
        highscoreText.text = "Highscore: " + score.ToString();
    }

    public void LivesChange(int lives)
    {
        if (lives < 0) lives = 0; // Temporary fix for lingering bullets after death
        healthDisplay.sprite = healthStates[lives];
    }

    // Toggle Game Over and hide in-game UI
    public void ShowGameOver()
    {
        // Toggle Visibility
        SetCanvasGroupState(inGameUI, false);
        SetCanvasGroupState(gameOverUI, true);
    }

    // Resets UI states
    public void ResetUI()
    {
        SetCanvasGroupState(inGameUI, true);
        SetCanvasGroupState(gameOverUI, false);

        inGameScoreText.text = "Score: 0";
        gameOverScoreText.text = inGameScoreText.text;
        healthDisplay.sprite = healthStates[gameConstants.playerMaxLives];
    }
}
