using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    // External components
    public CanvasGroup inGameUI;
    public CanvasGroup gameOverUI;
    public TextMeshProUGUI inGameScoreText;
    public TextMeshProUGUI gameOverScoreText;

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

    public void ShowObjective()
    {
        //
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
    }
}
