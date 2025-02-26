using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI highscoreText;
    public PersistentIntVariable gameScore;

    public void ShowHighScore()
    {
        highscoreText.text = "TOP- " + gameScore.highestValue.ToString("D6");
        highscoreText.gameObject.SetActive(true);
    }
}
