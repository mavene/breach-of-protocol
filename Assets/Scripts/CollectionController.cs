using UnityEngine;

public class CollectionController : MonoBehaviour
{

    private ScoreController scorer;

    // Start is called before the first frame update
    void Start()
    {
        scorer = GameObject.Find("Scorer").GetComponent<ScoreController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            scorer.UpdateScore();
            Destroy(gameObject);
        }
    }
}
