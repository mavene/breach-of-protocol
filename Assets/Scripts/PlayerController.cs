using UnityEngine;

// Player FSM
public enum PlayerState
{
    Idle,
    Run,
    Shoot,
    Die
};

public class PlayerController : MonoBehaviour
{
    // Position
    private Vector3 startPosition;

    // Movement
    public float moveSpeed = 5f;
    private Rigidbody2D playerBody;
    private Vector2 moveInput;

    // Attack
    public GameObject bulletPrefab;
    public float bulletSpeed;
    private float lastFire;
    public float fireDelay;
    private Vector2 attackInput;

    // Animation
    public Animator playerAnimator;

    // UI
    // TODO: Shift this part to overall GameController
    public UIManager uiManager;
    private ScoreController scorer;

    // Game State
    // TODO: Shift this part to overall GameController
    public GameObject enemies;
    public GameObject items;

    public PlayerState currentState = PlayerState.Idle;
    //add audio for gameover
    public AudioSource audioSource;
    public AudioClip gameOverSound;

    void Start()
    {
        startPosition = transform.localPosition;

        playerBody = GetComponent<Rigidbody2D>();
        playerBody.constraints = RigidbodyConstraints2D.FreezeRotation; //disallow rotation especially after colliding

        playerAnimator = GetComponent<Animator>();
        scorer = GameObject.Find("Scorer").GetComponent<ScoreController>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleAnimations();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        if ((attackInput.x != 0 || attackInput.y != 0) && Time.time > lastFire + fireDelay)
        {
            //if attackInput is going to the top or bottom, shoot vertically
            if (attackInput.y > 0)
            {
                Shoot(attackInput,"up");
            }
            else if(attackInput.y < 0)
            {
                Shoot(attackInput,"down");
            }
            else if(attackInput.x > 0)
            {
               Shoot(attackInput,"right");
            }
            else if(attackInput.x < 0)
            {
                Shoot(attackInput,"left");
            }

            
        }
    }

    private void HandleInput()
    {
        // Store movement input
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        // Store attack input
        attackInput = new Vector2(Input.GetAxis("ShootingHorizontal"), Input.GetAxis("ShootingVertical"));

        // Check player states after input processing
        if (attackInput.x != 0 || attackInput.y != 0)
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
                playerAnimator.Play("player-death");
                break;
        }
    }

    private void ApplyMovement()
    {
        // Apply translation
        playerBody.velocity = moveInput * moveSpeed;

        // Set animation parameter -> TODO: check if I really need this and how to vary the animation using this param
        // playerAnimator.SetFloat("xSpeed", Mathf.Abs(playerBody.velocity.x));
    }

    // TODO: Instantiate bullet a bit lower to match gun position of player
    private void Shoot(Vector2 direction, string orientation)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(
            (direction.x < 0) ? Mathf.Floor(direction.x) * bulletSpeed : Mathf.Ceil(direction.x) * bulletSpeed,
            (direction.y < 0) ? Mathf.Floor(direction.y) * bulletSpeed : Mathf.Ceil(direction.y) * bulletSpeed,
            0
        );
        if(orientation == "up")
        {
            bullet.transform.Rotate(0,0,90);
        }
        else if(orientation == "down")
        {
            bullet.transform.Rotate(0,0,-90);
        }
        
        Debug.Log("Bullet fired in direction: " + direction);
        
        lastFire = Time.time;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            audioSource.PlayOneShot(gameOverSound);
            Time.timeScale = 0.0f;
            scorer.FinaliseScore();
            uiManager.ShowGameOver();
        }
    }

    // TODO: Shift these out to GameController to control start/end game states
    public void RestartButtonCallback()
    {
        ResetGame();
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // Reset player
        playerBody.transform.localPosition = startPosition;
        //faceRightState = true;

        // Reset scores
        scorer.ResetScore();

        // Reset items and enemies
        ResetItems();
        ResetEnemies();

        // Hide Game Over Screen
        uiManager.ResetUI();
    }

    // TODO: Move out to GameController
    private void ResetItems()
    {
        
        foreach (Transform eachChild in items.transform)
        {
            //if chestcontroller is found, respawn chest
            if (eachChild.GetComponent<ChestController>() != null)
            {
                eachChild.GetComponent<ChestController>().Respawn();
            }
            else{
                eachChild.GetComponent<ItemController>().Respawn();
            }
            Debug.Log("Respawning item: " + eachChild.name);
            
        }
    }

    // TODO: Move out to GameController
    // TODO: I know its repeated but in case enemies need to respawn differently I can split the logic
    private void ResetEnemies()
    {
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.GetComponent<EnemyController>().Respawn();
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyController>().startPosition;
        }
    }


}
