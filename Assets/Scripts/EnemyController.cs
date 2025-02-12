using System.Collections;
using UnityEngine;

// Enemy FSM 
public enum EnemyState
{
    Idle,
    Chase,
    Die
};

public class EnemyController : MonoBehaviour
{
    // State
    public EnemyState currentState = EnemyState.Idle;
    public Vector3 startPosition;
    public float range = 5f;

    // Movement
    public float speed = 1f;
    private Rigidbody2D enemyBody;
    private bool choosingDirection = false;
    private Vector3 randomDirection;

    // Animation
    public Animator enemyAnimator;
    private bool isDeathAnimationStarted = false;

    // Audio
    public AudioSource audioSource;
    public AudioClip chaseActiveSound;
    public AudioClip chaseInactiveSound;
    public AudioClip deathSound;
    private bool firstTrigger = true;

    // Player interactions
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;

        enemyBody = GetComponent<Rigidbody2D>();
        enemyBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        enemyAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        HandleStates();
        HandleAnimations();
    }

    private void HandleStates()
    {
        if (currentState == EnemyState.Die)
        {
            return;
        }

        if (IsPlayerInRange(range) && currentState != EnemyState.Die)
        {
            currentState = EnemyState.Chase;

            //audioSource.PlayOneShot(chaseActiveSound);
        }
        else
        {
            currentState = EnemyState.Idle;
            //audioSource.PlayOneShot(chaseActiveSound);
        }

        switch (currentState)
        {
            case (EnemyState.Idle):
                Wander();
                break;
            case (EnemyState.Chase):
                Chase();
                break;
        }
    }

    private void HandleAnimations()
    {
        switch (currentState)
        {
            case (EnemyState.Idle):
                enemyAnimator.Play("enemy-idle");
                break;
            case (EnemyState.Chase):
                enemyAnimator.Play("enemy-chase");
                break;
            case (EnemyState.Die):
                if (!isDeathAnimationStarted)
                {
                    isDeathAnimationStarted = true;
                    enemyAnimator.SetBool("isDead", currentState == EnemyState.Die);
                    StartCoroutine(HideDelay());
                }
                break;
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

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    public void Die()
    {
        currentState = EnemyState.Die;
        enemyBody.velocity = Vector2.zero;
        isDeathAnimationStarted = false;
        //StartCoroutine(HideDelay());
        //gameObject.SetActive(false);
        //Destroy(gameObject); // Rationale for removing this is I want to use SetActive instead
    }

    public void Respawn()
    {
        currentState = EnemyState.Idle;
        isDeathAnimationStarted = false;
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

    private IEnumerator HideDelay()
    {
        while (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("enemy-death"))
        {
            yield return null;
        }

        audioSource.PlayOneShot(deathSound);
        yield return new WaitForSeconds(deathSound.length);

        while (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
