using UnityEngine;
using System.Collections;
using System;
public class ChestController : MonoBehaviour
{
    private Animator animator;
    private ScoreController scorer;
    // Position
    private Vector3 startPosition;
    //audio for rewards
    public AudioSource audioSource;
    public AudioClip rewardSound;
    //audio for hit chest
    public AudioClip hitSound;

    public float minX = -14f;
    public float maxX = 2.5f;
    public float minY = -4f;
    public float maxY = 4f;
    void Start()
    {
        startPosition = transform.localPosition;
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject.");
        }
        //setups audio source
        audioSource = GetComponent<AudioSource>();

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name);
        Debug.Log("Triggered by: " + other.gameObject.tag);
        if (other.gameObject.CompareTag("Bullet"))
        {
         Debug.Log("Bullet hit chest");   
         audioSource.PlayOneShot(hitSound);
            animator.SetBool("Open", true);
            StartCoroutine(DelayedAction());
        }
        if (other.gameObject.CompareTag("Player"))
        {
         //when animator rewards is true, the chest will open
            if (animator.GetBool("rewards"))
            {
                //add audio for rewards
                audioSource.PlayOneShot(rewardSound);
                scorer = GameObject.Find("Scorer").GetComponent<ScoreController>();
                scorer.UpdateScore5();
                animator.SetBool("Open", false);       
                animator.SetBool("rewards", false);
                StartCoroutine(RespawnChest());
                // gameObject.SetActive(false);
                GetComponent<Renderer>().enabled = false;
            }
        }
    }
    private IEnumerator DelayedAction()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(0.3f);

        // Perform the action after 1 second
        Debug.Log("1 second has passed!");
        animator.SetBool("rewards", true);
        
    }
    public void Respawn()
    {
        GetComponent<Renderer>().enabled = false;
        Debug.Log("Respawning chest");
        transform.localPosition = startPosition;
        if (animator != null)
    {
        animator.SetBool("Open", false);       // Reset animation if it was playing "Open" animation
        animator.SetBool("rewards", false);    // Reset rewards state to false
        GetComponent<Renderer>().enabled = true;
    }

    Debug.Log("Chest respawned with reset animation and position.");
    }
    private IEnumerator RespawnChest()
    {
        // Wait for 1 second before respawning
        yield return new WaitForSeconds(1.0f);

        // Enable the chest and set it to a random position
        // gameObject.SetActive(true);
        
        if (animator != null)
            {
                animator.SetBool("Open", false);       // Reset animation if it was playing "Open" animation
                animator.SetBool("rewards", false);    // Reset rewards state to false
            }
        // Set the chest to a random position within the defined range
        float randomX = UnityEngine.Random.Range(minX, maxX);
        float randomY = UnityEngine.Random.Range(minY, maxY);
        //print x and y
        Debug.Log("Random X: " + randomX + ", Random Y: " + randomY);
        transform.position = new Vector3(randomX, randomY, 0);
        GetComponent<Renderer>().enabled = true;

        // Reset animation parameters
        if (animator != null)
        {
            animator.SetBool("Open", false);       // Reset animation if it was playing "Open" animation
            animator.SetBool("rewards", false);    // Reset rewards state to false
        }

        Debug.Log("Chest respawned at random position.");
    }
}