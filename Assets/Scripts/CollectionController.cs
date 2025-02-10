using UnityEngine;

public class CollectionController : MonoBehaviour
{

    // Scoring
    private ScoreController scorer;

    // Animation
    public Animator collectibleAnimator;

    // Start is called before the first frame update
    void Start()
    {
        scorer = GameObject.Find("Scorer").GetComponent<ScoreController>();
        collectibleAnimator = GetComponent<Animator>();
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
            scorer.UpdateScore();
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
    }

    private void HandleAnimations()
    {
        collectibleAnimator.Play("item-idle");
    }
}