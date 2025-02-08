using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public TextMeshProUGUI inGameScoreText;
    public TextMeshProUGUI gameOverScoreText;

    [System.NonSerialized]
    public int score = 0; // Hide from inspector

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    public void UpdateScore()
    {
        score++;
        inGameScoreText.text = "Score: " + score.ToString();
    }

    public void FinaliseScore()
    {
        gameOverScoreText.text = "Score: " + score.ToString();
    }

    public void ResetScore()
    {
        score = 0;
    }
}
