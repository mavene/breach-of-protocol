using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PersistentIntVariable gameScore;
    public static GameManager instance;
    public GameObject pauseIcon;  // Assign Pause Sprite GameObject
    public GameObject resumeIcon; // Assign Resume Sprite GameObject
    private bool isPaused = false;
    private void Start()
    {
        gameScore.SetValue(0); // Reset game score on start
        resumeIcon.SetActive(false);
    }
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        // Detect if the player clicked on pause or resume button
        if (Input.GetMouseButtonDown(0)) // Detect left mouse click or touch
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == pauseIcon)
                {
                    Debug.Log("Pause button clicked!");
                    TogglePause();
                }
                else if (hit.collider.gameObject == resumeIcon)
                {
                    Debug.Log("Resume button clicked!");
                    TogglePause();
                }
            }
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Debug.Log("Game is Paused.");
        Time.timeScale = 0;  // Stop the game
        isPaused = true;

        pauseIcon.SetActive(false);  // Hide Pause Icon
        resumeIcon.SetActive(true);  // Show Resume Icon
    }

    public void ResumeGame()
    {
        Debug.Log("Game is Resumed.");
        Time.timeScale = 1;  // Resume the game
        isPaused = false;

        pauseIcon.SetActive(true);   // Show Pause Icon
        resumeIcon.SetActive(false); // Hide Resume Icon
    }

    public void StartRespawnPowerupCoroutine(PowerupItemController powerupItemController, float delay)
    {
        StartCoroutine(RespawnPowerup(powerupItemController, delay));
    }

    private IEnumerator RespawnPowerup(PowerupItemController powerupItemController, float delay)
    {
        Debug.Log("Power-up will respawn in " + delay + " seconds...");

        for (int i = (int)delay; i > 0; i--)
        {
            Debug.Log("Respawning in: " + i + " seconds");
            yield return new WaitForSeconds(1f);
        }

        // Generate a new random position
        powerupItemController.Respawn();
    }   
    public void IncreaseScore(int amount)
    {
        gameScore.ApplyChange(amount);
    }

    public void GameOver()
    {
        Debug.Log("Game Over! High Score: " + gameScore.highestValue);
    }
}
