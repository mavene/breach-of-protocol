using UnityEngine;
using UnityEngine.Events;

public class ItemController : MonoBehaviour
{
    // Events
    public UnityEvent<int> onItemPickup;

    // Position
    private Vector3 startPosition;

    // Animation
    public Animator itemAnimator;

    // Audio
    public AudioSource audioSource;
    public AudioClip rewardSound;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;

        itemAnimator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // TODO: Add different reactions based on item type -> blue is score and red is health
        HandleAnimations();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(rewardSound);
            onItemPickup.Invoke(1); // Calls UpdateScore (Score)
            gameObject.SetActive(false);
        }
    }

    private void HandleAnimations()
    {
        itemAnimator.Play("item-idle");
    }

    // #------------------- GAME -------------------#

    public void Respawn()
    {
        gameObject.SetActive(true);
        gameObject.transform.localPosition = startPosition;
    }
}