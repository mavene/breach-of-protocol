using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
    // Events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent gameOver;

    // State
    private int currentScene = 1;
    private string currentEnvironment = "Lab";

    // External components
    public GameObject enemies;
    public GameObject items;

    // Start is called before the first frame update
    void Start()
    {
        gameStart.Invoke(); // Call ShowObjective (UI)
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameRestart()
    {
        ResetItems();
        ResetEnemies();
        gameRestart.Invoke(); // Calls ResetScore (Score), ResetUI (UI), ResetPlayer (Player), ResetHotwire (Hotwire), ResetDoor (Door)
        Time.timeScale = 1.0f;
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver.Invoke(); // Calls UpdateHighScore (Scorer), ShowGameOver (UI)
    }

    private void ResetItems()
    {
        foreach (Transform eachChild in items.transform)
        {
            if (eachChild.GetComponent<ChestController>() != null)
            {
                eachChild.GetComponent<ChestController>().Respawn();
            }
            else
            {
                eachChild.GetComponent<ItemController>().Respawn();
            }
        }
    }

    private void ResetEnemies()
    {
        foreach (Transform eachChild in enemies.transform)
        {
            if (eachChild.GetComponent<EnemyController>() != null)
            {
                eachChild.GetComponent<EnemyController>().Respawn();
                eachChild.transform.localPosition = eachChild.GetComponent<EnemyController>().startPosition;
            }
            else if (eachChild.GetComponent<SlimeController>() != null)
            {
                eachChild.GetComponent<SlimeController>().Respawn();
                eachChild.transform.localPosition = eachChild.GetComponent<SlimeController>().startPosition;
            }
        }
    }

    public void LoadNextScene()
    {
        if (currentScene == 1 && currentEnvironment == "Lab")
        {
            currentScene += 1;
            SceneManager.LoadScene(currentEnvironment + "-" + currentScene.ToString());
        }
    }
}
