using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    public enum PlayerState
    {
        Idle,
        Run,
        Shoot,
        Die
    };

    public PlayerState currentState = PlayerState.Idle;

    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerBody.constraints = RigidbodyConstraints2D.FreezeRotation; //disallow rotation especially after colliding

        playerAnimator = GetComponent<Animator>();
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
            Shoot(attackInput);
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

    private void ApplyMovement()
    {
        // Apply translation
        playerBody.velocity = moveInput * moveSpeed;

        // Set animation parameter -> TODO: check if I really need this and how to vary the animation using this param
        // playerAnimator.SetFloat("xSpeed", Mathf.Abs(playerBody.velocity.x));
    }

    private void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(
            (direction.x < 0) ? Mathf.Floor(direction.x) * bulletSpeed : Mathf.Ceil(direction.x) * bulletSpeed,
            (direction.y < 0) ? Mathf.Floor(direction.y) * bulletSpeed : Mathf.Ceil(direction.y) * bulletSpeed,
            0
        );
        lastFire = Time.time;
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
}
