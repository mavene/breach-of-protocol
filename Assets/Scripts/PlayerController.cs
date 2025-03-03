using System.Collections;
using UnityEngine;

// Player FSM
public enum PlayerState
{
    Idle,
    Run,
    Shoot,
    Deflect,
    Die
};

public class PlayerController : MonoBehaviour
{
    // Player Components
    private Rigidbody2D playerBody;
    private SpriteRenderer playerSprite;

    // State
    public PlayerState currentState = PlayerState.Idle;

    // Conditionals
    public bool faceRightState = true;
    private bool isDeathStarted = false;

    // Movement
    private Vector3 startPosition;
    public float moveSpeed = 5f;
    private Vector2 moveInput;

    // Attack
    public GameObject bulletPrefab;
    public float bulletSpeed = 2.5f;
    public float fireDelay = 1f;
    private float lastFire;
    private Vector2 attackInput;

    // Deflect
    public float deflectRange = 2f;
    public float deflectArcAngle = 100f;
    public float deflectCooldown = 0.5f;
    private float lastDeflectTime;
    private Vector2 lastMoveDirection;
    private Vector2 aimDirection;

    // Animation
    private Animator playerAnimator;

    // TODO -> Try to refactor into observers
    private UIManager uiManager;
    private ScoreController scorer;
    public AudioSource audioSource;
    public AudioClip gameOverSound;
    public GameObject enemies;
    public GameObject items;

    void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        playerBody = GetComponent<Rigidbody2D>();
        playerBody.constraints = RigidbodyConstraints2D.FreezeRotation; //disallow rotation especially after colliding

        startPosition = transform.localPosition;

        playerAnimator = GetComponent<Animator>();
        playerAnimator.keepAnimatorStateOnDisable = true; // prevents sprite from resetting to a weird frame from previous death

        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        scorer = GameObject.Find("Scorer").GetComponent<ScoreController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleStates();
        HandleAnimations();
        HandleAim();
    }

    void FixedUpdate()
    {
        if (currentState != PlayerState.Die)
        {
            Move(moveInput);
        }
    }

    // #-------------------- HANDLERS ---------------------#
    private void HandleStates()
    {
        // Check player states after input processing
        if (currentState == PlayerState.Die)
        {
            return;
        }
        else if (attackInput.x != 0 || attackInput.y != 0)
        {
            currentState = PlayerState.Shoot;
        }
        else if (moveInput.x != 0 || moveInput.y != 0)
        {
            currentState = PlayerState.Run;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    private void HandleAnimations()
    {
        switch (currentState)
        {
            case (PlayerState.Idle):
                playerAnimator.Play("player-idle");
                break;
            case (PlayerState.Run):
                playerAnimator.Play("player-run");
                break;
            case (PlayerState.Shoot):
                playerAnimator.Play("player-shoot");
                break;
            case (PlayerState.Die):
                // This conditional prevents loops since we are deactivating the gameObject
                if (!isDeathStarted)
                {
                    isDeathStarted = true;
                    playerAnimator.Play("player-death");
                    //StartCoroutine(PlayGameOverAudio());
                    StartCoroutine(HideDelay());
                }
                break;
        }
    }

    void HandleAim()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimDirection = (mousePos - (Vector2)transform.position).normalized;
    }

    // #-------------------- SUBSCRIBERS ------------------#
    // Subscriber - Movement
    public void MoveCheck(Vector2 value1, int value2)
    {
        if (currentState == PlayerState.Die)
        {
            moveInput = Vector2.zero;
        }
        moveInput = value1;
        lastMoveDirection = moveInput.normalized;

        if (moveInput.x != 0)  // Only flip if there's horizontal movement
        {
            FlipPlayerSprite(value2);
        }
        // Movement is continuous input so put in FixedUpdates
    }

    void FlipPlayerSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            playerSprite.flipX = true;

        }
        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            playerSprite.flipX = false;
        }
    }

    void Move(Vector2 value)
    {
        playerBody.velocity = value * moveSpeed;
    }

    // -----------------------------------------------------------------------

    // Subscriber - Attack
    public void AttackCheck(Vector2 value)
    {
        if (currentState == PlayerState.Die)
        {
            attackInput = Vector2.zero;
        }
        attackInput = value;

        // For some reason I need to put it here, if I shift it up to FixedUpdate its bollocks
        if (Time.time > lastFire + fireDelay)
        {
            Shoot(attackInput);
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
        bullet.tag = "PlayerProjectile";
        bullet.AddComponent<Rigidbody2D>();
        bullet.GetComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(
            (direction.x < 0) ? Mathf.Floor(direction.x) * bulletSpeed : Mathf.Ceil(direction.x) * bulletSpeed,
            (direction.y < 0) ? Mathf.Floor(direction.y) * bulletSpeed : Mathf.Ceil(direction.y) * bulletSpeed,
            0
        );
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.Rotate(0, 0, angle);
        lastFire = Time.time;
    }

    // Subscriber - Deflect
    public void DeflectCheck()
    {
        Debug.Log("Start deflect");
        if (currentState == PlayerState.Die || Time.time < lastDeflectTime + deflectCooldown)
            return;

        lastDeflectTime = Time.time;
        Vector2 arcDir = lastMoveDirection;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, deflectRange);

        foreach (Collider2D hit in hits)
        {
            IDeflectable projectile = hit.GetComponent<IDeflectable>();
            if (projectile == null) continue;

            if (hit.gameObject.CompareTag("DeflectedProjectile")) continue;

            Vector2 dirToProjectile = (hit.transform.position - transform.position).normalized;

            float angle = Vector2.Angle(arcDir, dirToProjectile);

            if (angle <= deflectArcAngle / 2)
            {
                hit.gameObject.tag = "DeflectedProjectile";
                projectile.Deflect(aimDirection);
            }
        }
        Debug.Log("End deflect");
    }

    // #------------------- TRIGGERS -------------------#
    // TODO: TEMP DISABLED FOR DEFLECTS
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyProjectile"))
        {
            currentState = PlayerState.Die;
        }
    }

    // #------------------ COROUTINES --------------------
    private IEnumerator HideDelay()
    {
        while (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("player-death"))
        {
            yield return null;
        }

        audioSource.PlayOneShot(gameOverSound);
        yield return new WaitForSeconds(gameOverSound.length);

        while (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        StopGame();
        gameObject.SetActive(false);
    }

    // #------------------- GAME -------------------#
    private void StopGame()
    {
        // audioSource.PlayOneShot(gameOverSound);
        Time.timeScale = 0.0f;
        scorer.FinaliseScore();
        uiManager.ShowGameOver();
    }

    public void RestartButtonCallback()
    {
        ResetGame();
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // Reset player
        currentState = PlayerState.Idle;
        isDeathStarted = false;
        gameObject.SetActive(true);
        playerBody.transform.localPosition = startPosition;
        //playerAnimator.Update(0f);

        // Reset scores
        scorer.ResetScore();

        // Reset items and enemies
        ResetItems();
        ResetEnemies();

        // Hide Game Over Screen
        uiManager.ResetUI();
    }

    private void ResetItems()
    {
        foreach (Transform eachChild in items.transform)
        {
            // eachChild.GetComponent<ItemController>().Respawn();
            if (eachChild.GetComponent<ChestController>() != null)
            {
                eachChild.GetComponent<ChestController>().Respawn();
            }
            else
            {
                eachChild.GetComponent<ItemController>().Respawn();
            }
        }
    }

    private void ResetEnemies()
    {
        foreach (Transform eachChild in enemies.transform)
        {
            if (eachChild.GetComponent<EnemyController>() != null)
            {
                eachChild.GetComponent<EnemyController>().Respawn();
                eachChild.transform.localPosition = eachChild.GetComponent<EnemyController>().startPosition;
            }
            else if (eachChild.GetComponent<SlimeController>() != null)
            {
                eachChild.GetComponent<SlimeController>().Respawn();
                eachChild.transform.localPosition = eachChild.GetComponent<SlimeController>().startPosition;
            }
        }
    }
}