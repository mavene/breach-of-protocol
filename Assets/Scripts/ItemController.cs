using UnityEngine;

public class ItemController : MonoBehaviour
{

    // Position
    private Vector3 startPosition;

    // Scoring
    private ScoreController scorer;

    // Animation
    public Animator itemAnimator;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;

        scorer = GameObject.Find("Scorer").GetComponent<ScoreController>();
        itemAnimator = GetComponent<Animator>();
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
        gameObject.transform.localPosition = startPosition;
    }

    private void HandleAnimations()
    {
        itemAnimator.Play("item-idle");
    }
}