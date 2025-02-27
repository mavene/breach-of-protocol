using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private AudioSource audioSource;
    public AudioClip backgroundMusic;
    public PersistentIntVariable gameScore;

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
            Debug.Log("🎵 Background music stopped.");
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