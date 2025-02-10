using System.Collections;
using UnityEngine;

// Enemy FSM 
public enum EnemyState
{
    Wander,
    Follow,
    Die
};

public class EnemyController : MonoBehaviour
{
    // State
    public EnemyState currentState = EnemyState.Wander;
    public Vector3 startPosition;
    public float range = 5f;

    // Movement
    public float speed = 1f;
    private Rigidbody2D enemyBody;
    private bool choosingDirection = false;
    private Vector3 randomDirection;

    // Player interactions
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Track original orientation for respawn
        startPosition = transform.localPosition;

        enemyBody = GetComponent<Rigidbody2D>();
        enemyBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case (EnemyState.Wander):
                Wander();
                break;
            case (EnemyState.Follow):
                Follow();
                break;
            case (EnemyState.Die):
                Die();
                break;
        }

        if (IsPlayerInRange(range) && currentState != EnemyState.Die)
        {
            currentState = EnemyState.Follow;
        }
        else
        {
            currentState = EnemyState.Wander;
        }
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    private void Wander()
    {
        // Keep choosing random directions
        if (!choosingDirection)
        {
            StartCoroutine(ChooseDirection());
        }

        // Actually move in chosen direction
        transform.position += -transform.right * speed * Time.deltaTime;
    }

    private void Follow()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    public void Die()
    {
        currentState = EnemyState.Die;
        gameObject.SetActive(false);
        //Destroy(gameObject); // Rationale for removing this is I want to use SetActive instead
    }

    public void Respawn()
    {
        currentState = EnemyState.Wander;
        gameObject.SetActive(true);
        gameObject.transform.localPosition = startPosition;
    }

    private IEnumerator ChooseDirection()
    {
        choosingDirection = true;
        yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 8f));
        randomDirection = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
        Quaternion nextRotation = Quaternion.Euler(randomDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, UnityEngine.Random.Range(0.5f, 2.5f));
        choosingDirection = false;
    }
}
