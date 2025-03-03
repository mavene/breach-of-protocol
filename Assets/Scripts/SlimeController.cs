using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum SlimeState
{
    Idle,
    Chase,
    Die
};

public class SlimeController : MonoBehaviour
{
    // Events
    public UnityEvent<int> onSlimeDeath;

    // State
    public SlimeState currentState = SlimeState.Idle;
    public Vector3 startPosition;
    public float range = 5f;

    // Movement
    public float speed = 1f;
    private Rigidbody2D slimeBody;
    public Vector2 topLeft;
    public Vector2 topRight;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;
    private Vector2[] path; // Stores the ordered path
    private int currentTargetIndex = 0; // Index of the current corner target
    private SpriteRenderer spriteRenderer;

    // Animation
    public Animator slimeAnimator;
    private bool isDeathAnimationStarted = false;
    public RuntimeAnimatorController enemyProjAnimController;

    // Attack
    public GameObject SlimeAttackPrefab;
    public float bulletSpeed;
    public float fireRate;
    private float lastFireTime;
    private float lastFire;

    // Audio
    public AudioSource audioSource;
    public AudioClip chaseActiveSound;
    public AudioClip chaseInactiveSound;
    public AudioClip deathSound;

    // Player interactions
    private GameObject player;

    public ScoreController scorer;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;

        slimeBody = GetComponent<Rigidbody2D>();
        slimeBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        slimeAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player");
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get sprite renderer

        // Define path in order (clockwise movement)
        path = new Vector2[] { topLeft, topRight, bottomRight, bottomLeft };

        // Start at top-left corner
        transform.position = topRight;
    }

    // Update is called once per frame
    void Update()
    {
        HandleStates();
        HandleAnimations();
    }
    void FixedUpdate()
    {
        // Ensure player is still valid before accessing it
        if (player == null)
        {
            Debug.LogError("❌ ERROR: Player reference is NULL! Cannot shoot.");
            return;
        }

        if (currentState == SlimeState.Chase)
        {
            // Check if enough time has passed since last shot
            if (Time.time >= lastFireTime + fireRate)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;

                // Ensure direction is valid before shooting
                if (direction != Vector2.zero)
                {
                    Shoot(direction);
                    lastFireTime = Time.time; // Reset cooldown
                }
                else
                {
                    Debug.LogWarning("⚠ Warning: Attempted to shoot but direction is zero.");
                }
            }
        }
    }

    public void Respawn()
    {
        currentState = SlimeState.Idle;
        transform.position = topLeft;
        gameObject.SetActive(true);
        gameObject.transform.localPosition = startPosition;
    }
    private void HandleStates()
    {
        if (currentState == SlimeState.Die)
        {
            return;
        }

        if (IsPlayerInRange(range) || currentState == SlimeState.Chase)
        {
            currentState = SlimeState.Chase;
            Chase();

            //audioSource.PlayOneShot(chaseActiveSound);
        }
        else
        {
            currentState = SlimeState.Idle;
            Wander();
            //audioSource.PlayOneShot(chaseActiveSound);
        }

    }

    private void HandleAnimations()
    {
        switch (currentState)
        {
            case (SlimeState.Idle):
                slimeAnimator.Play("slime-idle");
                break;
            case (SlimeState.Chase):
                slimeAnimator.Play("slime-idle");
                break;
            case (SlimeState.Die):
                if (!isDeathAnimationStarted)
                {
                    isDeathAnimationStarted = true;
                    slimeAnimator.SetBool("isDead", currentState == SlimeState.Die);
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
        Vector2 targetPosition = path[currentTargetIndex]; // Get current target position

        // Move toward the current target
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Flip sprite when moving left or right
        if (targetPosition.x < transform.position.x)
        {
            spriteRenderer.flipX = true; // Moving left
        }
        else if (targetPosition.x > transform.position.x)
        {
            spriteRenderer.flipX = false; // Moving right
        }

        // If reached the target, move to the next point
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % path.Length; // Loop through points
        }

        // If the slime leaves the path, correct its position
        if (!IsOnPath(transform.position))
        {
            SnapToNearestCorner();
        }
    }

    private bool IsOnPath(Vector2 position)
    {
        // Check if within the rectangle bounds
        return position.x >= bottomLeft.x && position.x <= bottomRight.x &&
               position.y >= bottomLeft.y && position.y <= topLeft.y;
    }

    private void SnapToNearestCorner()
    {
        float minDistance = float.MaxValue;
        int closestIndex = 0;

        // Find the closest corner
        for (int i = 0; i < path.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, path[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        // Move to the nearest corner and resume movement from there
        transform.position = path[closestIndex];
        currentTargetIndex = (closestIndex + 1) % path.Length;
    }

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        if (player.transform.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true; // Moving left
        }
        else if (player.transform.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false; // Moving right
        }
    }

    public void Die()
    {
        currentState = SlimeState.Die;
        slimeBody.velocity = Vector2.zero;
        isDeathAnimationStarted = false;
        StartCoroutine(HideDelay());
        onSlimeDeath.Invoke(1);
    }
    private IEnumerator HideDelay()
    {
        slimeAnimator.Play("slime-death");
        yield return new WaitForSeconds(0.6f);
        gameObject.SetActive(false);
    }

    private void Shoot(Vector2 direction)
    {
        if (SlimeAttackPrefab == null)
        {
            Debug.LogError("❌ ERROR: SlimeAttackPrefab is NULL! Cannot instantiate.");
            return;
        }
        GameObject attack = Instantiate(SlimeAttackPrefab, transform.position, transform.rotation) as GameObject;
        attack.tag = "EnemyProjectile";
        attack.GetComponent<Animator>().runtimeAnimatorController = enemyProjAnimController;
        attack.GetComponent<Animator>().Play("bullet-player");

        attack.AddComponent<Rigidbody2D>().gravityScale = 0;
        attack.GetComponent<Rigidbody2D>().velocity = new Vector3(
            direction.x * bulletSpeed,
            direction.y * bulletSpeed,
            0
        );
        lastFire = Time.time;
        Destroy(attack, 1f);

    }

}
