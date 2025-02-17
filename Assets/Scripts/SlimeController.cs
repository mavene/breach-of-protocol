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

    // Animation
    public Animator slimeAnimator;
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

        slimeBody = GetComponent<Rigidbody2D>();
        slimeBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        slimeAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player");
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get sprite renderer

        // Define path in order (clockwise movement)
        path = new Vector2[] { topLeft, topRight, bottomRight, bottomLeft };

        // Start at top-left corner
        transform.position = topLeft;
    }

    // Update is called once per frame
    void Update()
    {
        HandleStates();
        HandleAnimations();
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
                    // StartCoroutine(HideDelay());
                }
                break;
        }
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    // private void Wander()
    // {

    //     // Actually move in chosen direction
    //     transform.position += -transform.right * speed * Time.deltaTime;
    // }
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
        //StartCoroutine(HideDelay());
        //gameObject.SetActive(false);
        //Destroy(gameObject); // Rationale for removing this is I want to use SetActive instead
    }

}
