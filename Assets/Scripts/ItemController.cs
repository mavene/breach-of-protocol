using UnityEngine;

public class ItemController : MonoBehaviour
{

    // Position
    private Vector3 startPosition;

    // Scoring
    private ScoreController scorer;

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

        scorer = GameObject.Find("Scorer").GetComponent<ScoreController>();
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
            scorer.UpdateScore(1);
            gameObject.SetActive(false);
        }
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        gameObject.transform.localPosition = startPosition;
    }

    private void HandleAnimations()
    {
        itemAnimator.Play("item-idle");
    }
}