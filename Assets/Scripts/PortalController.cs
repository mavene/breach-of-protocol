using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered the portal: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Player entered portal, loading Jamie 1...");
            GameManager.instance.LoadNextScene("Jamie 1"); // Ensure scene name is correct
        }
    }
}
