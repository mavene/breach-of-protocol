using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public CanvasGroup inGameUI;
    public CanvasGroup gameOverUI;

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
