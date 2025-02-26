using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject pauseIcon;  // Assign Pause Icon Sprite GameObject
    public GameObject resumeIcon; // Assign Resume Icon Sprite GameObject

    private bool isPaused = false;

    void Start()
    {
        resumeIcon.SetActive(false); // Ensure Resume icon is hidden at start
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detect Mouse Click or Touch
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == pauseIcon)
                {
                    TogglePause();
                }
                else if (hit.collider.gameObject == resumeIcon)
                {
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
        Time.timeScale = 0;  // Stop the game
        isPaused = true;

        pauseIcon.SetActive(false);  // Hide Pause Icon
        resumeIcon.SetActive(true);  // Show Resume Icon
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;  // Resume the game
        isPaused = false;

        pauseIcon.SetActive(true);   // Show Pause Icon
        resumeIcon.SetActive(false); // Hide Resume Icon
    }
}
