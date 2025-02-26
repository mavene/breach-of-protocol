using UnityEngine;
using System.Collections;

public class PowerupItemController : MonoBehaviour
{
    private Vector3 startPosition;
    private PlayerController playerController;

    public Animator itemAnimator;
    public AudioSource audioSource;
    public AudioClip powerupSound;
    private SpriteRenderer spriteRenderer;
    private Collider2D itemCollider;

    void Start()
    {
        startPosition = transform.localPosition;
        itemAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCollider = GetComponent<Collider2D>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("PowerupItemController: No Player found!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
{
    if (other.gameObject.CompareTag("Player"))
    {
        Debug.Log("✅ Player picked up power-up!");

        // ✅ Play sound at player's location to ensure it's heard
        if (powerupSound != null)
        {
            AudioSource.PlayClipAtPoint(powerupSound, Camera.main.transform.position, 1.0f); // Play at full volume near the camera
            Debug.Log("✅ Power-up sound played globally!");
        }
        else
        {
            Debug.LogError("❌ PowerupItemController: powerupSound is missing!");
        }

        playerController.ActivateSpeedBoost();

        GameManager.instance.StartRespawnPowerupCoroutine(this, 8f);
        gameObject.SetActive(false);
    }
}



    public void Restart()
    {
        gameObject.SetActive(true);
        gameObject.transform.localPosition = startPosition;
        Debug.Log("Power-up respawned!");
    }
    public void Respawn()
{
    // Generate a random position within a specific range
    float randomX = Random.Range(-14f, 2.5f);
    float randomY = Random.Range(-4f, 4f);
    Vector3 randomPosition = startPosition + new Vector3(randomX, randomY, 0);

    gameObject.transform.localPosition = randomPosition; // Move to random position
    gameObject.SetActive(true); // Reactivate power-up

    Debug.Log("Power-up respawned at: " + randomPosition);
}

}