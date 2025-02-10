using System.Collections;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;

public enum EnemyState
{
    Wander,
    Follow,
    Die
};

// TODO: He spins weirdly now... lmao
public class EnemyController : MonoBehaviour
{
    private GameObject player;

    public EnemyState currentState = EnemyState.Wander;

    public Vector3 startPosition;
    public float range = 5f;
    public float speed = 1f;

    private bool choosingDirection = false;
    private bool dead = false;
    private Vector3 randomDirection;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startPosition = transform.position;
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
        Destroy(gameObject);
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
