using System.Collections;
using UnityEngine;

// Enemy FSM 
public enum EnemyState
{
    Idle,
    Chase,
    Shoot,
    Die
};

public class EnemyController : MonoBehaviour
{
    // State
    public EnemyState currentState = EnemyState.Idle;
    public Vector3 startPosition;
    public float chaseRange = 5f;
    public float shootRange = 0.2f;

    // Movement
    public float speed = 1f;
    private Rigidbody2D enemyBody;
    private bool choosingDirection = false;
    private Vector3 randomDirection;

    // Attack
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    private float lastFire;
    public float fireDelay = 0.7f;

    // Animation
    public Animator enemyAnimator;
    public RuntimeAnimatorController enemyProjAnimController;
    private bool isDeathAnimationStarted = false;

    // Audio
    public AudioSource audioSource;
    public AudioClip chaseActiveSound;
    public AudioClip chaseInactiveSound;
    public AudioClip deathSound;

    // Player interactions
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;

        enemyBody = GetComponent<Rigidbody2D>();
        enemyBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        enemyAnimator = GetComponent<Animator>();
        enemyAnimator.keepAnimatorStateOnDisable = true;
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
        else if (IsPlayerInRange(chaseRange))
        {
            currentState = EnemyState.Chase;
            if (IsPlayerInRange(shootRange))
            {
                currentState = EnemyState.Shoot;
            }
        }
        else
        {
            currentState = EnemyState.Idle;
        }

        switch (currentState)
        {
            case (EnemyState.Idle):
                Wander();
                break;
            case (EnemyState.Chase):
                Chase();
                break;
            case (EnemyState.Shoot):
                if (Time.time > lastFire + fireDelay)
                {
                    Shoot();
                }
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
            case (EnemyState.Shoot):
                enemyAnimator.Play("enemy-shoot");
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
        return Vector3.Distance(transform.position, player.transform.position) <= chaseRange;
    }

    private void Wander()
    {
        //audioSource.PlayOneShot(chaseInactiveSound);
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
        //audioSource.PlayOneShot(chaseActiveSound);
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
        bullet.tag = "EnemyProjectile";
        bullet.GetComponent<Animator>().runtimeAnimatorController = enemyProjAnimController;
        bullet.GetComponent<Animator>().Play("bullet-player");

        Vector2 direction = (player.transform.position - transform.position).normalized;
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.Rotate(0, 0, angle);
        lastFire = Time.time;
    }

    // For now, enemies die instantly
    // public void Damage(float damageAmount)
    // {
    //     currentHealth -= damageAmount;
    //     HealthCheck();
    //     // Do some UI updates here
    // }

    // private void HealthCheck()
    // {
    //     if (currentHealth <= 0)
    //     {
    //         currentState = EnemyState.Die;
    //     }
    // }

    public void Die()
    {
        currentState = EnemyState.Die;
        enemyBody.velocity = Vector2.zero;
        isDeathAnimationStarted = false;
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
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        randomDirection = new Vector3(0, 0, Random.Range(0, 360));
        Quaternion nextRotation = Quaternion.Euler(randomDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 2.5f));
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
