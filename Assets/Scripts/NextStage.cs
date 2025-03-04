using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NextStage : MonoBehaviour
{
    // Events
    public UnityEvent onLoadScene;
    public UnityEvent<Vector2> blockPlayer;

    // External components
    private Collider2D doorCollider;

    // Conditionals
    private bool isOpen = false;

    // Animation
    private Animator doorAnimator;

    // Start is called before the first frame update
    void Start()
    {
        doorCollider = GetComponent<Collider2D>();

        doorAnimator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        doorAnimator.SetBool("isOpen", true);
        StartCoroutine(PlayDoorAnimation());
    }

    // #------------------- TRIGGERS -------------------#
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpen && other.CompareTag("Player"))
        {
            // Push the player back slightly (block)
            Vector2 pushDirection = (other.transform.position - transform.position).normalized;
            blockPlayer.Invoke(pushDirection); // Calls BlockCheck (Player)
        }
        else if (isOpen && other.CompareTag("Player"))
        {
            onLoadScene.Invoke(); // Calls LoadNextScene (Game)
        }
    }

    // #------------------ COROUTINES -------------------- 
    private IEnumerator PlayDoorAnimation()
    {
        while (!doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("door-open"))
            yield return null;

        // Wait for animation to complete
        while (doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return null;
    }

    // #------------------- GAME -------------------#
    public void ResetDoor()
    {
        // Reset animation
        isOpen = false;
        doorAnimator.SetBool("isOpen", isOpen);
        doorAnimator.Play("door-idle", 0, 0f);
    }
}
