using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PersistentIntVariable gameScore;
    public static GameManager instance;
    public GameObject pauseIcon;  // Assign Pause Sprite GameObject
    public GameObject resumeIcon; // Assign Resume Sprite GameObject
    private AudioSource audioSource;
    public AudioClip backgroundMusic;
    private bool isPaused = false;
    private void Start()
    {
        gameScore.SetValue(0); // Reset game score on start
        resumeIcon.SetActive(false);
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ✅ Ensures GameManager persists
        }
        else
        {
            Destroy(gameObject); // ✅ Prevents duplicate GameManager instances
            return;
        }

        // ✅ Ensure only ONE AudioSource exists
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        if (audioSource.clip == null) // ✅ Prevents multiple background music overlaps
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
    }    void Update()
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
    public void LoadNextScene(string sceneName)
    {
        Debug.Log("Loading Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
    public void StopMusic()
    {   
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Background music stopped.");
        }
    }

    public void ResetGame()
    {
        Debug.Log("🔄 Resetting game and restarting background music...");

        // ✅ Stop current music before scene reload
        StopMusic();

        // Reset game score
        gameScore.SetValue(0);

        // ✅ Load original scene
        LoadNextScene("Jamie");

        // ✅ Restart background music after scene loads
        StartCoroutine(RestartMusicAfterSceneLoad());
    }

private IEnumerator RestartMusicAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.5f); // ✅ Wait for the scene to load

        if (audioSource != null && backgroundMusic != null)
        {
            if (!audioSource.isPlaying) // ✅ Prevents music from overlapping
            {
                audioSource.Stop();
                audioSource.clip = backgroundMusic;
                audioSource.loop = true;
                audioSource.volume = 0.5f;
                audioSource.Play();
                Debug.Log("🎵 Background music restarted from the beginning.");
            }
            else
            {
                Debug.Log("⚠ Background music is already playing. Skipping restart.");
            }
        }
    }


}
