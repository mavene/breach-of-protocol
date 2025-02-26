using System.Collections;
using UnityEngine;

public enum SlimeState
{
    Idle,
    Chase,
    Die
};

public class SlimeController : MonoBehaviour
{
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
    public GameObject portal;

    // Animation
    public Animator slimeAnimator;
    private bool isDeathAnimationStarted = false;
    public RuntimeAnimatorController enemyProjAnimController;

    // Attack
    public GameObject SlimeAttackPrefab;
    public float bulletSpeed;
    public float fireRate;
    private float lastFireTime;
    private Vector2 attackInput;
    private float lastFire;

    // Audio
    public AudioSource audioSource;
    public AudioClip chaseActiveSound;
    public AudioClip chaseInactiveSound;
    public AudioClip deathSound;

    // Player interactions
    private GameObject player;

    public ScoreController scorer;
    private float movementSoundCooldown = 1.0f; // Cooldown between movement sounds
    private float lastMovementSoundTime = 0f;

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

    // Check if player is dead
    PlayerController playerController = player.GetComponent<PlayerController>();
    if (playerController != null && playerController.currentState == PlayerState.Die)
    {
        // Debug.Log("⚠ Player is dead, Slime stops shooting.");
        slimeBody.velocity = Vector2.zero;
        return; // Stop shooting if the player is dead
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
        // isDeathAnimationStarted = false;
        transform.position = topLeft;
        gameObject.SetActive(true);
        gameObject.transform.localPosition = startPosition;
        enabled = true;
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

    private void Wander()
    {
        Vector2 targetPosition = path[currentTargetIndex]; // Get current target position

        // Move toward the current target
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Flip sprite when moving left or right
        spriteRenderer.flipX = targetPosition.x < transform.position.x;

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

        // ✅ Play movement sound every `movementSoundCooldown` seconds
        if (audioSource != null && chaseActiveSound != null)
        {
            if (Time.time >= lastMovementSoundTime + movementSoundCooldown)
            {
                audioSource.PlayOneShot(chaseActiveSound);
                lastMovementSoundTime = Time.time; // Reset cooldown timer
            }
        }
    }

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        spriteRenderer.flipX = player.transform.position.x < transform.position.x;

        // ✅ Play movement sound every `movementSoundCooldown` seconds
        if (audioSource != null && chaseActiveSound != null)
        {
            if (Time.time >= lastMovementSoundTime + movementSoundCooldown)
            {
                audioSource.PlayOneShot(chaseActiveSound);
                lastMovementSoundTime = Time.time; // Reset cooldown timer
            }
        }
    }

    public void Die()
    {
        currentState = SlimeState.Die;
        slimeBody.velocity = Vector2.zero;
        isDeathAnimationStarted = false;
        StartCoroutine(HideDelay());
        scorer.UpdateScore(3);
        //gameObject.SetActive(false);
        //Destroy(gameObject); // Rationale for removing this is I want to use SetActive instead
        // Stop movement sound if playing
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Play death sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        if (portal != null)
        {
            portal.SetActive(true);
            Debug.Log("Portal activated!");
        }
        else{
            Debug.Log("Portal is NULL!");
        }
    }
    private IEnumerator HideDelay()
    {
        slimeAnimator.Play("slime-death");
        yield return new WaitForSeconds(0.6f);
        gameObject.SetActive(false);
        // Destroy(gameObject);
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
    public void StopSlimeMovement()
    {
        Debug.Log("Slime stopped moving because player died.");
        currentState = SlimeState.Idle; // Change state to Idle
        slimeBody.velocity = Vector2.zero; // Stop movement
        slimeBody.isKinematic = true; // Disable physics
        enabled = false;
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }


}
