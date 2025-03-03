using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

public class ChestController : MonoBehaviour
{
    // Events
    public UnityEvent<int> onChestOpen;

    // Position
    private Vector3 startPosition;

    // Animator
    private Animator animator;

    // Audio
    public AudioSource audioSource;
    public AudioClip rewardSound;
    public AudioClip hitSound;

    // Chest variables
    private float minX = -14f;
    private float maxX = 2.5f;
    private float minY = -4f;
    private float maxY = 4f;

    void Start()
    {
        startPosition = transform.localPosition;

        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

    }
    //the chest will move from left to right with a range of +-0.5
    void Update()
    {
        if (animator.GetBool("isOpen") == false)
        {
            float x = Mathf.PingPong(Time.time, 1) - 0.5f;
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            Destroy(other.gameObject);
            audioSource.PlayOneShot(hitSound);
            animator.SetBool("isOpen", true);
            StartCoroutine(DelayedAction());
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (animator.GetBool("reward"))
            {
                audioSource.PlayOneShot(rewardSound);
                onChestOpen.Invoke(2);
                animator.SetBool("isOpen", false);
                animator.SetBool("reward", false);
                StartCoroutine(RespawnChest());
                GetComponent<Renderer>().enabled = false;
            }
        }
    }
    private IEnumerator DelayedAction()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(0.3f);

        // Perform the action after 1 second
        animator.SetBool("reward", true);

    }
    public void Respawn()
    {
        GetComponent<Renderer>().enabled = false;
        transform.localPosition = startPosition;
        if (animator != null)
        {
            animator.SetBool("isOpen", false);       // Reset animation if it was playing "Open" animation
            animator.SetBool("reward", false);    // Reset rewards state to false
            GetComponent<Renderer>().enabled = true;
        }
    }
    private IEnumerator RespawnChest()
    {
        // Wait for 1 second before respawning
        yield return new WaitForSeconds(1.0f);

        if (animator != null)
        {
            animator.SetBool("isOpen", false);       // Reset animation if it was playing "Open" animation
            animator.SetBool("reward", false);    // Reset rewards state to false
        }
        // Set the chest to a random position within the defined range
        float randomX = UnityEngine.Random.Range(minX, maxX);
        float randomY = UnityEngine.Random.Range(minY, maxY);
        transform.position = new Vector3(randomX, randomY, 0);
        GetComponent<Renderer>().enabled = true;

        // Reset animation parameters
        if (animator != null)
        {
            animator.SetBool("isOpen", false);       // Reset animation if it was playing "Open" animation
            animator.SetBool("reward", false);    // Reset rewards state to false
        }
    }
}